using CSharpFunctionalExtensions;
using GestionAgenda.Models;

namespace GestionAgenda.Services;

/// <summary>
/// Casos de uso de negocio de la agenda de contactos. Esta es la interfaz
/// que consume Program.cs; nunca habla directamente con el repositorio.
/// </summary>
public interface IContactoService
{
    Result<Contacto> ObtenerPorId(int id);
    Result<Contacto> ObtenerPorAlias(string alias);
    Result<Contacto> Buscar(string? texto, int pagina, int tamanoPagina);
    Result<Contacto> CrearContacto(string nombre, string telefono, string email, string alias);
    Result<Contacto> ActualizarContacto(int id, string nombre, string telefono, string email, string alias);
    Result EliminarContacto(int id);
}