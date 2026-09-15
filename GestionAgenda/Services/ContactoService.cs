using GestionAgenda.Errors;
using GestionAgenda.Models;
using GestionAgenda.Repositories;
using Serilog;
using CSharpFunctionalExtensions;
using GestionAgenda.Cache;
using GestionAgenda.Validators;

namespace GestionAgenda.Services;

/// <summary>
/// Orquesta las reglas de negocio (validaciones) y delega la persistencia
/// en IContactoRepository. Esta clase SOLO conoce la abstraccion
/// IContactoRepository, nunca EF Core ni SQLite directamente (DIP).
///
/// Responsabilidad unica (SRP): esta clase se encarga solo de validar y
/// orquestar; no sabe nada de como se guardan ni de como se cachean los
/// contactos, de eso se encargan otras clases (carpetas Repositories y Cache).
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
            var error = new ContactoErrors(CodigoResultado.PeticionInvalida, "El Id debe ser mayor que 0.");
            return Result.Failure<Contacto>(error.ToString());
        }

        return repository.GetById(id);
    }

    public Result<Contacto> GetByAlias(string alias)
    {
        _logger.Debug("Solicitando contacto por Alias={Alias}", alias);

        if (string.IsNullOrWhiteSpace(alias))
        {
            _logger.Warning("GetByAlias rechazado: alias vacio");
            var error = new ContactoErrors(CodigoResultado.PeticionInvalida, "El alias no puede estar vacio.");
            return Result.Failure<Contacto>(error.ToString());
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
            var error = new ContactoErrors(CodigoResultado.PeticionInvalida, "Pagina y tamanoPagina deben ser mayores que 0.");
            return Result.Failure<IEnumerable<Contacto>>(error.ToString());
        }

        var contactos = repository.GetAll(texto, pagina, tamanoPagina).ToList();
        return Result.Success<IEnumerable<Contacto>>(contactos);
    }

    public Result<Contacto> CreateContacto(string nombre, string telefono, string email, string alias)
    {
        var validacion = ValidarDatos(nombre, telefono, email, alias);
        if (validacion.IsFailure)
        {
            _logger.Warning("CreateContacto rechazado por validacion: {Error}", validacion.Error);
            return Result.Failure<Contacto>(validacion.Error);
        }

        var contacto = new Contacto
        {
            Nombre = nombre.Trim(),
            Telefono = telefono.Trim(),
            Email = email.Trim(),
            Alias = alias.Trim()
        };

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
            var error = new ContactoErrors(CodigoResultado.PeticionInvalida, "El Id debe ser mayor que 0.");
            return Result.Failure<Contacto>(error.ToString());
        }

        var validacion = ValidarDatos(nombre, telefono, email, alias);
        if (validacion.IsFailure)
        {
            _logger.Warning("UpdateContacto rechazado por validacion. Id={Id}, Error={Error}", id, validacion.Error);
            return Result.Failure<Contacto>(validacion.Error);
        }

        var contacto = new Contacto
        {
            Id = id,
            Nombre = nombre.Trim(),
            Telefono = telefono.Trim(),
            Email = email.Trim(),
            Alias = alias.Trim()
        };

        var resultado = repository.Update(contacto);

        if (resultado.IsSuccess)
            _logger.Information("Contacto actualizado desde el servicio. Id={Id}", id);

        return resultado;
    }

    public Result DeleteContacto(int id)
    {
        if (id <= 0)
        {
            _logger.Warning("DeleteContacto rechazado: Id invalido ({Id})", id);
            var error = new ContactoErrors(CodigoResultado.PeticionInvalida, "El Id debe ser mayor que 0.");
            return Result.Failure(error.ToString());
        }

        var resultado = repository.Delete(id);

        if (resultado.IsSuccess)
            _logger.Information("Contacto eliminado desde el servicio. Id={Id}", id);

        return resultado;
    }

    private Result ValidarDatos(string nombre, string telefono, string email, string alias)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return Result.Failure(new ContactoErrors(CodigoResultado.PeticionInvalida, "El nombre no puede estar vacio.").ToString());

        if (string.IsNullOrWhiteSpace(telefono))
            return Result.Failure(new ContactoErrors(CodigoResultado.PeticionInvalida, "El telefono no puede estar vacio.").ToString());

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return Result.Failure(new ContactoErrors(CodigoResultado.PeticionInvalida, "El email no es valido.").ToString());

        if (string.IsNullOrWhiteSpace(alias))
            return Result.Failure(new ContactoErrors(CodigoResultado.PeticionInvalida, "El alias no puede estar vacio.").ToString());

        return Result.Success();
    }
}