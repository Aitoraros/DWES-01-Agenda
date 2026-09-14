using GestionAgenda.Models;

namespace GestionAgenda.Factory;

public static class ContactosFactory
{
    public static IEnumerable<Contacto> Seed()
    {
        var fecha = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

        return new List<Contacto>
        {
            new Contacto
            {
                Id = 1, Nombre = "Carlos Mendoza", Telefono = "+34612345678", Email = "carlos.mendoza@example.com",
                Alias = "Charlie", IsDeleted = false, CreatedAt = fecha.AddDays(-60), UpdatedAt = fecha.AddDays(-10)
            },
            new Contacto
            {
                Id = 2,
                Nombre = "Lucía Gómez", Telefono = "+34623456789", Email = "lucia.gomez@example.com",
                Alias = "Lu", IsDeleted = false, CreatedAt = fecha.AddDays(-55), UpdatedAt = fecha.AddDays(-55)
            },
            new Contacto
            {
                Id = 3,
                Nombre = "Alejandro Ruiz", Telefono = "+34634567890", Email = "alex.ruiz@example.com",
                Alias = "Alex", IsDeleted = false, CreatedAt = fecha.AddDays(-50), UpdatedAt = fecha.AddDays(-5)
            },
            new Contacto
            {
                Id = 4, Nombre = "María Fernández", Telefono = "+34645678901", Email = "maria.fernandez@example.com",
                Alias = string.Empty, IsDeleted = true, CreatedAt = fecha.AddDays(-48), UpdatedAt = fecha.AddDays(-2)
            },
            new Contacto
            {
                Id = 5,
                Nombre = "Javier López", Telefono = "+34656789012", Email = "javi.lopez@example.com", Alias = "Javi",
                IsDeleted = false, CreatedAt = fecha.AddDays(-45), UpdatedAt = fecha.AddDays(-45)
            },
            new Contacto
            {
                Id = 6,
                Nombre = "Sofía Martínez", Telefono = "+34667890123", Email = "sofia.martinez@example.com",
                Alias = "Sofi", IsDeleted = false, CreatedAt = fecha.AddDays(-40), UpdatedAt = fecha.AddDays(-12)
            },
            new Contacto
            {
                Id = 7, Nombre = "Diego Torres", Telefono = "+34678901234", Email = "diego.torres@example.com",
                Alias = string.Empty, IsDeleted = false, CreatedAt = fecha.AddDays(-38), UpdatedAt = fecha.AddDays(-38)
            },
            new Contacto
            {
                Id = 8, Nombre = "Elena Navarro", Telefono = "+34689012345", Email = "elena.navarro@example.com",
                Alias = "Nena", IsDeleted = false, CreatedAt = fecha.AddDays(-35), UpdatedAt = fecha.AddDays(-20)
            },
            new Contacto
            {
                Id = 9, Nombre = "Adrián Sánchez", Telefono = "+34690123456", Email = "adrian.sanchez@example.com",
                Alias = "Adri", IsDeleted = true, CreatedAt = fecha.AddDays(-30), UpdatedAt = fecha.AddDays(-1)
            },
            new Contacto
            {
                Id = 10, Nombre = "Laura Morales", Telefono = "+34601234567", Email = "laura.morales@example.com",
                Alias = string.Empty, IsDeleted = false, CreatedAt = fecha.AddDays(-28), UpdatedAt = fecha.AddDays(-28)
            },
            new Contacto
            {
                Id = 11,
                Nombre = "Pablo Castro", Telefono = "+34611223344", Email = "pablo.castro@example.com",
                Alias = "Pablito", IsDeleted = false, CreatedAt = fecha.AddDays(-25), UpdatedAt = fecha.AddDays(-15)
            },
            new Contacto
            {
                Id = 12,
                Nombre = "Carmen Ramos", Telefono = "+34622334455", Email = "carmen.ramos@example.com",
                Alias = string.Empty, IsDeleted = false, CreatedAt = fecha.AddDays(-22), UpdatedAt = fecha.AddDays(-22)
            },
            new Contacto
            {
                Id = 13,
                Nombre = "Daniel Ortega", Telefono = "+34633445566", Email = "dan.ortega@example.com",
                Alias = "Dani", IsDeleted = false, CreatedAt = fecha.AddDays(-20), UpdatedAt = fecha.AddDays(-3)
            },
            new Contacto
            {
                Id = 14,
                Nombre = "Ana Belén Gil", Telefono = "+34644556677", Email = "anabelen.gil@example.com",
                Alias = "Ana", IsDeleted = false, CreatedAt = fecha.AddDays(-18), UpdatedAt = fecha.AddDays(-18)
            },
            new Contacto
            {
                Id = 15, Nombre = "Hugo Serrano", Telefono = "+34655667788", Email = "hugo.serrano@example.com",
                Alias = string.Empty, IsDeleted = true, CreatedAt = fecha.AddDays(-15), UpdatedAt = fecha.AddDays(-7)
            },
            new Contacto
            {
                Id = 16,
                Nombre = "Paula Blanco", Telefono = "+34666778899", Email = "paula.blanco@example.com", Alias = "Pau",
                IsDeleted = false, CreatedAt = fecha.AddDays(-12), UpdatedAt = fecha.AddDays(-12)
            },
            new Contacto
            {
                Id = 17,
                Nombre = "Marcos Molina", Telefono = "+34677889900", Email = "marcos.molina@example.com",
                Alias = "Marc", IsDeleted = false, CreatedAt = fecha.AddDays(-10), UpdatedAt = fecha.AddDays(-4)
            },
            new Contacto
            {
                Id = 18, Nombre = "Irene Vidal", Telefono = "+34688990011", Email = "irene.vidal@example.com",
                Alias = string.Empty, IsDeleted = false, CreatedAt = fecha.AddDays(-8), UpdatedAt = fecha.AddDays(-8)
            },
            new Contacto
            {
                Id = 19, Nombre = "Gonzalo Marín", Telefono = "+34699001122", Email = "gonzalo.marin@example.com",
                Alias = "Gonza", IsDeleted = false, CreatedAt = fecha.AddDays(-5), UpdatedAt = fecha.AddDays(-1)
            },
            new Contacto
            {
                Id = 20,
                Nombre = "Marta Iglesias", Telefono = "+34600112233", Email = "marta.iglesias@example.com",
                Alias = "Marti", IsDeleted = false, CreatedAt = fecha.AddDays(-2), UpdatedAt = fecha.AddDays(-2)
            }
        };
    }
}