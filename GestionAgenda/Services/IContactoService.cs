using CSharpFunctionalExtensions;
using GestionAgenda.Models;

namespace GestionAgenda.Services;

/// <summary>
/// Casos de uso de negocio de la agenda de contactos. Esta es la interfaz
/// que consume Program.cs; nunca habla directamente con el repositorio.
/// </summary>
public interface IContactoService
{
    Result<Contacto> GetById(int id);
    Result<Contacto> GetByAlias(string alias);
    Result<IEnumerable<Contacto>> GetAll(string? texto, int pagina, int tamanoPagina);
    Result<Contacto> CreateContacto(string nombre, string telefono, string email, string alias);
    Result<Contacto> UpdateContacto(int id, string nombre, string telefono, string email, string alias);
    Result DeleteContacto(int id);
}