using GestionAgenda.Entity;
using GestionAgenda.Models;

namespace GestionAgenda.Mapper;

public static class ContactoMapper
{
    public static ContactoEntity ToEntity(this Contacto model)
    {
        return new ContactoEntity
        {
            Id = model.Id,
            Nombre = model.Nombre,
            Alias = model.Alias,
            Email = model.Email,
            Telefono = model.Telefono,
            IsDeleted = model.IsDeleted,
            CreatedAt = model.CreatedAt == default ? DateTime.UtcNow : model.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };
    }


    public static Contacto ToModel(this ContactoEntity entity)
    {
        return new Contacto
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Alias = entity.Alias,
            Email = entity.Email,
            Telefono = entity.Telefono,
            IsDeleted = entity.IsDeleted,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}