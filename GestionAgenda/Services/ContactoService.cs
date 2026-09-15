using GestionAgenda.Errors;
using GestionAgenda.Models;
using GestionAgenda.Repositories;
using Serilog;
using CSharpFunctionalExtensions;
using GestionAgenda.Cache;
using GestionAgenda.Validators;

namespace GestionAgenda.Services;

/// <summary>
/// Proporciona la lógica de negocio para la gestión integral de contactos, 
/// coordinando la validación de entidades, el acceso al repositorio y la gestión de caché.
/// </summary>

public class ContactoService(
    IContactoRepository repository,
    IValidador<Contacto> validador,
    ICache<int, Contacto> cache) : IContactoService
{
    private readonly ILogger _logger = Log.ForContext<ContactoService>();

    public Result<Contacto> GetById(int id)
    {
        _logger.Debug("Solicitando contacto por Id={Id}", id);

        if (id <= 0)
        {
            _logger.Warning("GetById rechazado: Id invalido ({Id})", id);
            return Result.Failure<Contacto>(
                new ContactoErrors(CodigoResultado.PeticionInvalida, "El Id debe ser mayor que 0.").ToString()
            );
        }

        var enCache = cache.Get(id);
        if (enCache is not null)
        {
            Console.WriteLine($"Id={id} servido desde memoria.");
            return Result.Success(enCache);
        }

        Console.WriteLine($"Id={id} no estaba en cache, se consulta SQLite.");
        var resultado = repository.GetById(id);

        if (resultado.IsSuccess)
            cache.Add(id, resultado.Value);

        return resultado;
    }

    public Result<Contacto> GetByAlias(string alias)
    {
        _logger.Debug("Solicitando contacto por Alias={Alias}", alias);

        if (string.IsNullOrWhiteSpace(alias))
        {
            _logger.Warning("GetByAlias rechazado: alias vacio");
            return Result.Failure<Contacto>(
                new ContactoErrors(CodigoResultado.PeticionInvalida, "El alias no puede estar vacio.").ToString()
            );
        }

        return repository.GetByAlias(alias);
    }

    public Result<IEnumerable<Contacto>> GetAll(string? texto, int pagina, int tamanoPagina)
    {
        _logger.Debug("Listando contactos. Texto={Texto}, Pagina={Pagina}, TamanoPagina={TamanoPagina}",
            texto, pagina, tamanoPagina);

        if (pagina <= 0 || tamanoPagina <= 0)
        {
            _logger.Warning("GetAll rechazado: paginacion invalida (Pagina={Pagina}, TamanoPagina={TamanoPagina})",
                pagina, tamanoPagina);
            return Result.Failure<IEnumerable<Contacto>>(
                new ContactoErrors(CodigoResultado.PeticionInvalida, "Pagina y tamanoPagina deben ser mayores que 0.").ToString()
            );
        }

        var contactos = repository.GetAll(texto, pagina, tamanoPagina).ToList();
        return Result.Success<IEnumerable<Contacto>>(contactos);
    }

    public Result<Contacto> CreateContacto(string nombre, string telefono, string email, string alias)
    {
        var contacto = new Contacto
        {
            Nombre = nombre.Trim(),
            Telefono = telefono.Trim(),
            Email = email.Trim(),
            Alias = alias.Trim()
        };

        var errores = validador.Validar(contacto).ToList();
        if (errores.Count > 0)
        {
            var mensaje = string.Join(" ", errores);
            _logger.Warning("CreateContacto rechazado por validacion: {Error}", mensaje);
            
            // Envolvemos los errores de validación en ContactoErrors con PeticionInvalida
            return Result.Failure<Contacto>(
                new ContactoErrors(CodigoResultado.PeticionInvalida, mensaje).ToString()
            );
        }

        var resultado = repository.Create(contacto);

        if (resultado.IsSuccess)
            _logger.Information("Contacto creado desde el servicio. Alias={Alias}", alias);

        return resultado;
    }

    public Result<Contacto> UpdateContacto(int id, string nombre, string telefono, string email, string alias)
    {
        if (id <= 0)
        {
            _logger.Warning("UpdateContacto rechazado: Id invalido ({Id})", id);
            return Result.Failure<Contacto>(
                new ContactoErrors(CodigoResultado.PeticionInvalida, "El Id debe ser mayor que 0.").ToString()
            );
        }

        var contacto = new Contacto
        {
            Id = id,
            Nombre = nombre.Trim(),
            Telefono = telefono.Trim(),
            Email = email.Trim(),
            Alias = alias.Trim()
        };

        var errores = validador.Validar(contacto).ToList();
        if (errores.Count > 0)
        {
            var mensaje = string.Join(" ", errores);
            _logger.Warning("UpdateContacto rechazado por validacion. Id={Id}, Error={Error}", id, mensaje);
            
            // Envolvemos los errores de validación en ContactoErrors con PeticionInvalida
            return Result.Failure<Contacto>(
                new ContactoErrors(CodigoResultado.PeticionInvalida, mensaje).ToString()
            );
        }

        var resultado = repository.Update(contacto);

        if (resultado.IsSuccess)
        {
            cache.Remove(id);
            _logger.Information("Contacto actualizado desde el servicio. Id={Id}", id);
        }

        return resultado;
    }

    public Result DeleteContacto(int id)
    {
        if (id <= 0)
        {
            _logger.Warning("DeleteContacto rechazado: Id invalido ({Id})", id);
            return Result.Failure(
                new ContactoErrors(CodigoResultado.PeticionInvalida, "El Id debe ser mayor que 0.").ToString()
            );
        }

        var resultado = repository.Delete(id);

        if (resultado.IsSuccess)
        {
            cache.Remove(id);
            _logger.Information("Contacto eliminado desde el servicio. Id={Id}", id);
        }

        return resultado;
    }
}