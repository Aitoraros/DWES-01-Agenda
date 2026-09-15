using CSharpFunctionalExtensions;
using GestionAgenda.Models;

namespace GestionAgenda.Repositories;

public interface IContactoRepository
{
    
    IEnumerable<Contacto> GetAll(string? texto, int pagina, int tamanoPagina);
    Result<Contacto> GetById(int id);
    Result<Contacto> GetByAlias(string alias);
    Result<Contacto> Create(Contacto contacto);
    Result<Contacto> Update(Contacto contacto);
    Result Delete(int id);
}