using CSharpFunctionalExtensions;
using GestionAgenda.Entity;
using GestionAgenda.Errors;
using GestionAgenda.Factory;
using GestionAgenda.Mapper;
using GestionAgenda.Models;
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

        if (seedData && !_context.Contactos.Any())
        {
            _logger.Information("Sembrando datos de contactos...");
            foreach (var c in ContactosFactory.Seed())
                Create(c);
        }
    }

    public IEnumerable<Contacto> GetAll(string? texto, int pagina, int tamanoPagina)
    {
        try
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

            return query
                .OrderBy(c => c.Nombre)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList()
                .Select(e => e.ToModel());
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error inesperado buscando contactos. Texto={Texto}, Pagina={Pagina}, TamanoPagina={TamanoPagina}",
                texto, pagina, tamanoPagina);
            return Enumerable.Empty<Contacto>();
        }
    }

    public Result<Contacto> GetById(int id)
    {
        try
        {
            var entity = _context.Contactos.FirstOrDefault(c => c.Id == id && !c.IsDeleted);

            if (entity is null)
            {
                _logger.Warning("No se encontro el contacto Id={Id}", id);
                var error = new ContactoErrors(CodigoResultado.NoEncontrado, $"No existe ningun contacto con Id={id}.");
                return Result.Failure<Contacto>(error.ToString());
            }

            return Result.Success(entity.ToModel());
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error inesperado obteniendo el contacto Id={Id}", id);
            var error = new ContactoErrors(CodigoResultado.ErrorInterno, $"Error inesperado accediendo a la base de datos: {ex.Message}");
            return Result.Failure<Contacto>(error.ToString());
        }
    }

    public Result<Contacto> GetByAlias(string alias)
    {
        try
        {
            var entity = _context.Contactos.FirstOrDefault(c => c.Alias.ToLower() == alias.ToLower() && !c.IsDeleted);

            if (entity is null)
            {
                _logger.Warning("No se encontro el contacto con alias={Alias}", alias);
                var error = new ContactoErrors(CodigoResultado.NoEncontrado, $"No existe ningun contacto con alias '{alias}'.");
                return Result.Failure<Contacto>(error.ToString());
            }

            return Result.Success(entity.ToModel());
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error inesperado obteniendo el contacto alias={Alias}", alias);
            var error = new ContactoErrors(CodigoResultado.ErrorInterno, $"Error inesperado accediendo a la base de datos: {ex.Message}");
            return Result.Failure<Contacto>(error.ToString());
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
                var error = new ContactoErrors(CodigoResultado.Conflicto, $"Ya existe un contacto con el alias '{contacto.Alias}'.");
                return Result.Failure<Contacto>(error.ToString());
            }

            var entity = contacto.ToEntity();
            _context.Contactos.Add(entity);
            _context.SaveChanges();

            _logger.Information("Contacto creado. Id={Id}, Alias={Alias}", entity.Id, entity.Alias);
            return Result.Success(entity.ToModel());
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error inesperado creando el contacto alias={Alias}", contacto.Alias);
            var error = new ContactoErrors(CodigoResultado.ErrorInterno, $"Error inesperado guardando el contacto: {ex.Message}");
            return Result.Failure<Contacto>(error.ToString());
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
                var error = new ContactoErrors(CodigoResultado.NoEncontrado, $"No existe ningun contacto con Id={contacto.Id}.");
                return Result.Failure<Contacto>(error.ToString());
            }

            var aliasEnUso = _context.Contactos.Any(c =>
                c.Id != contacto.Id && !c.IsDeleted && c.Alias.ToLower() == contacto.Alias.ToLower());
            if (aliasEnUso)
            {
                _logger.Warning("Alias duplicado al actualizar contacto Id={Id}: {Alias}", contacto.Id, contacto.Alias);
                var error = new ContactoErrors(CodigoResultado.Conflicto, $"Ya existe otro contacto con el alias '{contacto.Alias}'.");
                return Result.Failure<Contacto>(error.ToString());
            }

            
            existente.Nombre = contacto.Nombre;
            existente.Telefono = contacto.Telefono;
            existente.Email = contacto.Email;
            existente.Alias = contacto.Alias;
            existente.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            _logger.Information("Contacto actualizado. Id={Id}", existente.Id);
            return Result.Success(existente.ToModel());
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error inesperado actualizando el contacto Id={Id}", contacto.Id);
            var error = new ContactoErrors(CodigoResultado.ErrorInterno, $"Error inesperado actualizando el contacto: {ex.Message}");
            return Result.Failure<Contacto>(error.ToString());
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
                var error = new ContactoErrors(CodigoResultado.NoEncontrado, $"No existe ningun contacto con Id={id}.");
                return Result.Failure(error.ToString());
            }

           
            existente.IsDeleted = true;
            existente.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();

            _logger.Information("Contacto eliminado (soft delete). Id={Id}", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error inesperado eliminando el contacto Id={Id}", id);
            var error = new ContactoErrors(CodigoResultado.ErrorInterno, $"Error inesperado eliminando el contacto: {ex.Message}");
            return Result.Failure(error.ToString());
        }
    }
}