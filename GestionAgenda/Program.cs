using CSharpFunctionalExtensions;
using GestionAgenda.Cache;
using GestionAgenda.Config;
using GestionAgenda.Entity;
using GestionAgenda.Errors;
using GestionAgenda.Factory;
using GestionAgenda.Models;
using GestionAgenda.Repositories;
using GestionAgenda.Services;
using GestionAgenda.Validators;
using Microsoft.EntityFrameworkCore;
using Serilog;

// 1. Configuración del Logger (Serilog)
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(Enum.Parse<Serilog.Events.LogEventLevel>(AppConfig.LogMinimumLevel))
            .WriteTo.File(
                path: AppConfig.LogFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: AppConfig.LogRetainedFiles,
                outputTemplate: AppConfig.LogOutputTemplate)
            .CreateLogger();

        Log.Information("Iniciando la aplicación GestionAgenda...");

        // 2. Comprobación de directorio para la base de datos SQLite
        var connectionBuilder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(AppConfig.ConnectionString);
        var dbPath = connectionBuilder.DataSource;
        var directory = Path.GetDirectoryName(dbPath);
        
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 3. Inicialización y preparación de la Base de Datos
        var options = new DbContextOptionsBuilder<AgendaDbContext>()
            .UseSqlite(AppConfig.ConnectionString)
            .Options;

        using var context = new AgendaDbContext(options);
        context.Database.EnsureCreated();

        // Reinicio de datos con el Seed
        if (context.Contactos.Any())
        {
            context.Contactos.RemoveRange(context.Contactos);
            context.SaveChanges();
        }

        var entidades = ContactosFactory.Seed().Select(c => new ContactoEntity
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Telefono = c.Telefono,
            Email = c.Email,
            Alias = c.Alias,
            IsDeleted = c.IsDeleted,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });

        context.Contactos.AddRange(entidades);
        context.SaveChanges();
        Log.Information("Base de datos inicializada con {Count} contactos de prueba.", context.Contactos.Count());

        // 4. Inyección manual de dependencias
        IContactoRepository repository = new ContactoRepositoryEfCore(context);
        var validator = new ContactoValidator();
        var cache = new LruCache<int, Contacto>(AppConfig.CacheSize);
        IContactoService service = new ContactoService(repository, validator, cache);

        // 5. Ejecución del flujo de consola
        Console.WriteLine($"=== - Demostración de Agenda ===\n");

        // --- CONSULTAS Y PAGINACIÓN ---
        Console.WriteLine("--- 1. Listado de Contactos (Paginación y Filtros) ---");
        
        Console.WriteLine("\nMostrando Página 1 (5 por página):");
        ImprimirResultadoLista(service.GetAll(texto: null, pagina: 1, tamanoPagina: 5));

        Console.WriteLine("\nMostrando Página 2 (5 por página):");
        ImprimirResultadoLista(service.GetAll(texto: null, pagina: 2, tamanoPagina: 5));

        Console.WriteLine("\nFiltrando por el nombre o texto 'Gómez':");
        ImprimirResultadoLista(service.GetAll(texto: "Gómez", pagina: 1, tamanoPagina: 5));

        // --- BÚSQUEDAS ---
        Console.WriteLine("\n--- 2. Consultas de Contactos ---");

        Console.WriteLine("\nBuscando contacto con ID 2:");
        ImprimirResultado(service.GetById(2));

        Console.WriteLine("\nBuscando contacto con ID inexistente (999):");
        ImprimirResultado(service.GetById(999));

        Console.WriteLine("\nBuscando contacto por alias 'Charlie':");
        ImprimirResultado(service.GetByAlias("Charlie"));

        Console.WriteLine("\nBuscando contacto por alias inexistente 'Inexistente':");
        ImprimirResultado(service.GetByAlias("Inexistente"));

        // --- CREACIÓN ---
        Console.WriteLine("\n--- 3. Añadir Nuevos Contactos ---");

        Console.WriteLine("\nInsertando un nuevo contacto válido:");
        var resCrear = service.CreateContacto("Roberto Gómez", "+34699887766", "roberto.gomez@example.com", "Rober");
        ImprimirResultado(resCrear);

        Console.WriteLine("\nIntentando añadir un contacto con teléfono duplicado:");
        ImprimirResultado(service.CreateContacto("Duplicado Tlf", "+34612345678", "dup1@example.com", "Dup1"));

        Console.WriteLine("\nIntentando añadir un contacto con alias duplicado:");
        ImprimirResultado(service.CreateContacto("Duplicado Alias", "+34611111111", "dup2@example.com", "Charlie"));

        Console.WriteLine("\nIntentando añadir un contacto con datos no válidos:");
        ImprimirResultado(service.CreateContacto("", "123", "email_invalido", "X"));

        // --- ACTUALIZACIÓN ---
        Console.WriteLine("\n--- 4. Edición de Contactos ---");

        Console.WriteLine("\nActualizando datos del contacto con ID 1:");
        ImprimirResultado(service.UpdateContacto(1, "Carlos Mendoza Actualizado", "+34612345678", "carlos.mendoza.new@example.com", "CharlieEdit"));

        Console.WriteLine("\nIntentando actualizar un contacto que no existe (ID 999):");
        ImprimirResultado(service.UpdateContacto(999, "Inexistente", "+34600000000", "no@exist.com", "NoExist"));

        // --- ELIMINACIÓN ---
        Console.WriteLine("\n--- 5. Eliminación de Contactos ---");

        Console.WriteLine("\nEliminando el contacto con ID 1:");
        ImprimirResultadoVoid(service.DeleteContacto(1));

        Console.WriteLine("\nIntentando eliminar un contacto que no existe (ID 999):");
        ImprimirResultadoVoid(service.DeleteContacto(999));

        Log.Information("Demostración finalizada correctamente.");
    

    // --- MÉTODOS AUXILIARES DE CONSOLA ---

    void ImprimirResultado<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            Console.WriteLine($"  -> OK: {result.Value}");
        }
        else
        {
            ImprimirError(result.Error);
        }
    }

    void ImprimirResultadoLista(Result<IEnumerable<Contacto>> result)
    {
        if (result.IsSuccess)
        {
            var lista = result.Value.ToList();
            if (!lista.Any())
            {
                Console.WriteLine("  (No hay registros para mostrar)");
                return;
            }

            foreach (var c in lista)
            {
                Console.WriteLine($"  - [{c.Id}] {c.Nombre} | Tel: {c.Telefono} | Alias: {(string.IsNullOrEmpty(c.Alias) ? "Sin alias" : c.Alias)}");
            }
        }
        else
        {
            ImprimirError(result.Error);
        }
    }

    void ImprimirResultadoVoid(Result result)
    {
        if (result.IsSuccess)
        {
            Console.WriteLine("  -> Operación realizada con éxito.");
        }
        else
        {
            ImprimirError(result.Error);
        }
    }

    void ImprimirError(string error)
    {
        if (error is null) return;
        // Muestra directamente la salida formateada de ToString(), ej: [NoEncontrado] El recurso solicitado no existe.
        Console.WriteLine($"  -> Error {error}");
    }
