using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using GestionAgenda.Errors;
using GestionAgenda.Models;
using GestionAgenda.Repositories;

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
public class ContactoService : IContactoService
{
    private static readonly Regex RegexEmail = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
 
    private readonly IContactoRepository _repositorio;
 
    // Constructor
    public ContactoService(IContactoRepository repositorio)
    {
        _repositorio = repositorio;
    }
 
    public Result<Contacto> ObtenerPorId(int id)
    {
        if (id <= 0)
            return Result<Contacto>.Fallo(CodigoResultado.PeticionInvalida, "El Id debe ser un numero positivo.");
 
        return _repositorio.ObtenerPorId(id);
    }
 
    public Result<Contacto> ObtenerPorAlias(string alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
            return Result<Contacto>.Fallo(CodigoResultado.PeticionInvalida, "El alias no puede estar vacio.");
 
        return _repositorio.ObtenerPorAlias(alias.Trim());
    }
 
    public Result<ResultadoPaginado<Contacto>> Buscar(string? texto, int pagina, int tamanoPagina)
    {
        if (pagina <= 0)
            return Result<ResultadoPaginado<Contacto>>.Fallo(CodigoResultado.PeticionInvalida, "La pagina debe ser >= 1.");
 
        if (tamanoPagina <= 0 || tamanoPagina > 100)
            return Result<ResultadoPaginado<Contacto>>.Fallo(CodigoResultado.PeticionInvalida, "El tamano de pagina debe estar entre 1 y 100.");
 
        return _repositorio.Buscar(texto?.Trim(), pagina, tamanoPagina);
    }
 
    public Result<Contacto> CrearContacto(string nombre, string telefono, string email, string alias)
    {
        var errorValidacion = ValidarCampos(nombre, telefono, email, alias);
        if (errorValidacion is not null)
            return Result<Contacto>.Fallo(CodigoResultado.PeticionInvalida, errorValidacion);
 
        var contacto = new Contacto
        {
            Nombre = nombre.Trim(),
            Telefono = telefono.Trim(),
            Email = email.Trim(),
            Alias = alias.Trim()
        };
 
        return _repositorio.Crear(contacto);
    }
 
    public Result<Contacto> ActualizarContacto(int id, string nombre, string telefono, string email, string alias)
    {
        if (id <= 0)
            return Result<Contacto>.Fallo(CodigoResultado.PeticionInvalida, "El Id debe ser un numero positivo.");
 
        var errorValidacion = ValidarCampos(nombre, telefono, email, alias);
        if (errorValidacion is not null)
            return Result<Contacto>.Fallo(CodigoResultado.PeticionInvalida, errorValidacion);
 
        var contacto = new Contacto
        {
            Id = id,
            Nombre = nombre.Trim(),
            Telefono = telefono.Trim(),
            Email = email.Trim(),
            Alias = alias.Trim()
        };
 
        return _repositorio.Actualizar(contacto);
    }
 
    public Result EliminarContacto(int id)
    {
        if (id <= 0)
            return Result.Fallo(CodigoResultado.PeticionInvalida, "El Id debe ser un numero positivo.");
 
        return _repositorio.Eliminar(id);
    }
 
    /// <summary>
    /// Devuelve un mensaje de error si algun campo no es valido, o null si todo esta bien.
    /// </summary>
    private static string? ValidarCampos(string nombre, string telefono, string email, string alias)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return "El nombre es obligatorio.";
 
        if (string.IsNullOrWhiteSpace(telefono))
            return "El telefono es obligatorio.";
 
        if (string.IsNullOrWhiteSpace(alias))
            return "El alias es obligatorio.";
 
        if (string.IsNullOrWhiteSpace(email) || !RegexEmail.IsMatch(email.Trim()))
            return "El email no tiene un formato valido.";
        
        return null;
    }
}