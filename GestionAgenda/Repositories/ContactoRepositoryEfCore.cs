using CSharpFunctionalExtensions;
using GestionAgenda.Entity;
using GestionAgenda.Errors;
using GestionAgenda.Factory;
using GestionAgenda.Mapper;
using GestionAgenda.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GestionAgenda.Repositories;

public class ContactoRepositoryEfCore : IContactoRepository
{
    private readonly AgendaDbContext _context;
    private readonly ILogger _logger = Log.ForContext<ContactoRepositoryEfCore>();

    public ContactoRepositoryEfCore(AgendaDbContext context, bool dropData = false, bool seedData = false)
    {
        _context = context;
        
        if (dropData) _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        
        if (seedData && !_context.Contactos.Any()) {
            _logger.Information("Sembrando datos de contactos...");
            foreach (var c in ContactosFactory.Seed())
                Create(c);
        }
    }

    public IEnumerable<Contacto> GetAll(string? texto, int pagina, int tamanoPagina)
    {
        _logger.Debug("Buscando contactos. Texto={Texto}, Pagina={Pagina}, TamanoPagina={TamanoPagina}",
            texto, pagina, tamanoPagina);

        var query = _context.Contactos.Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var textoBusqueda = texto.ToLower();
            query = query.Where(c =>
                c.Nombre.ToLower().Contains(textoBusqueda) ||
                c.Alias.ToLower().Contains(textoBusqueda) ||
                c.Email.ToLower().Contains(textoBusqueda) ||
                c.Telefono.Contains(textoBusqueda));
        }

        // Primero ToList() (ejecuta el SQL) y LUEGO .Select(ToModel()): si el
        // Select fuera antes del ToList(), EF Core intentaria traducir
        // ToModel() a SQL y fallaria, porque no sabe convertir ese metodo
        // C# a una consulta de base de datos.
        return query
            .OrderBy(c => c.Nombre)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToList()
            .Select(e => e.ToModel());
    }

    public Result<Contacto> GetById(int id)
    {
        try
        {
            var entity = _context.Contactos.FirstOrDefault(c => c.Id == id && !c.IsDeleted);

            if (entity is null)
            {
                _logger.Warning("No se encontro el contacto Id={Id}", id);
                return new Result<Contacto>(false, null, CodigoResultado.NoEncontrado, $"No existe ningun contacto con Id={id}.");
            }

            return new Result<Contacto>(true, entity.ToModel(), CodigoResultado.Ok, "Contacto encontrado.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error inesperado obteniendo el contacto Id={Id}", id);
            return new Result<Contacto>(false, null, CodigoResultado.ErrorInterno, $"Error inesperado accediendo a la base de datos: {ex.Message}");
        }
    }

    public Result<Contacto> GetByAlias(string alias)
    {
        try
        {
            var entity = _context.Contactos.FirstOrDefault(c => c.Alias.ToLower() == alias.ToLower() && !c.IsDeleted);

            if (entity is null)
            {
                _logger.Warning("No se encontro el contacto alias={Alias}", alias);
                return new Result<Contacto>(false, null, CodigoResultado.NoEncontrado, $"No existe ningun contacto con alias '{alias}'.");
            }

            return new Result<Contacto>(true, entity.ToModel(), CodigoResultado.Ok, "Contacto encontrado.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error inesperado obteniendo el contacto alias={Alias}", alias);
            return new Result<Contacto>(false, null, CodigoResultado.ErrorInterno, $"Error inesperado accediendo a la base de datos: {ex.Message}");
        }
    }

    public Result<Contacto> Create(Contacto contacto)
    {
        try
        {
            var aliasEnUso = _context.Contactos.Any(c => !c.IsDeleted && c.Alias.ToLower() == contacto.Alias.ToLower());
            if (aliasEnUso)
            {
                _logger.Warning("Alias duplicado al crear contacto: {Alias}", contacto.Alias);
                return new Result<Contacto>(false, null, CodigoResultado.Conflicto, $"Ya existe un contacto con el alias '{contacto.Alias}'.");
            }

            var entity = contacto.ToEntity();
            _context.Contactos.Add(entity);
            _context.SaveChanges(); // aqui EF Core rellena entity.Id (autoincremental)

            _logger.Information("Contacto creado. Id={Id}, Alias={Alias}", entity.Id, entity.Alias);
            return new Result<Contacto>(true, entity.ToModel(), CodigoResultado.Creado, "Contacto creado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error inesperado creando el contacto alias={Alias}", contacto.Alias);
            return new Result<Contacto>(false, null, CodigoResultado.ErrorInterno, $"Error inesperado guardando el contacto: {ex.Message}");
        }
    }

    public Result<Contacto> Update(Contacto contacto)
    {
        try
        {
            var existente = _context.Contactos.FirstOrDefault(c => c.Id == contacto.Id && !c.IsDeleted);
            if (existente is null)
            {
                _logger.Warning("Intento de actualizar un contacto inexistente. Id={Id}", contacto.Id);
                return new Result<Contacto>(false, null, CodigoResultado.NoEncontrado, $"No existe ningun contacto con Id={contacto.Id}.");
            }

            var aliasEnUso = _context.Contactos.Any(c =>
                c.Id != contacto.Id && !c.IsDeleted && c.Alias.ToLower() == contacto.Alias.ToLower());
            if (aliasEnUso)
            {
                _logger.Warning("Alias duplicado al actualizar contacto Id={Id}: {Alias}", contacto.Id, contacto.Alias);
                return new Result<Contacto>(false, null, CodigoResultado.Conflicto, $"Ya existe otro contacto con el alias '{contacto.Alias}'.");
            }

            // Se actualiza "existente" (la entidad que EF ya rastrea) campo a
            // campo, en vez de crear una entidad nueva con ToEntity() y
            // llamar a Update(nuevaEntidad): si hicieramos eso, EF Core
            // lanzaria una excepcion porque ya hay una entidad con ese mismo
            // Id bajo seguimiento.
            existente.Nombre = contacto.Nombre;
            existente.Telefono = contacto.Telefono;
            existente.Email = contacto.Email;
            existente.Alias = contacto.Alias;
            existente.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            _logger.Information("Contacto actualizado. Id={Id}", existente.Id);
            return new Result<Contacto>(true, existente.ToModel(), CodigoResultado.Ok, "Contacto actualizado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error inesperado actualizando el contacto Id={Id}", contacto.Id);
            return new Result<Contacto>(false, null, CodigoResultado.ErrorInterno, $"Error inesperado actualizando el contacto: {ex.Message}");
        }
    }

    public Result Delete(int id)
    {
        try
        {
            var existente = _context.Contactos.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
            if (existente is null)
            {
                _logger.Warning("Intento de eliminar un contacto inexistente (o ya eliminado). Id={Id}", id);
                return new Result(false, CodigoResultado.NoEncontrado, $"No existe ningun contacto con Id={id}.");
            }

            // Borrado logico: se marca IsDeleted=true en vez de Remove(), para
            // conservar el historico. Por eso el resto de metodos filtran
            // "!IsDeleted".
            existente.IsDeleted = true;
            existente.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();

            _logger.Information("Contacto eliminado (soft delete). Id={Id}", id);
            return new Result(true, CodigoResultado.Ok, "Contacto eliminado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error inesperado eliminando el contacto Id={Id}", id);
            return new Result(false, CodigoResultado.ErrorInterno, $"Error inesperado eliminando el contacto: {ex.Message}");
        }
    }
}