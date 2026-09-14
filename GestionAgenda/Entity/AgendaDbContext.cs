using Microsoft.EntityFrameworkCore;

namespace GestionAgenda.Entity;

/// <summary>
/// Unico punto de contacto con EF Core / SQLite. Ninguna otra clase del
/// proyecto (aparte de EfContactoRepository, que la recibe inyectada)
/// conoce esta clase directamente.
/// </summary>
public class AgendaDbContext : DbContext
{
    
    private readonly string _connectionString;

    public AgendaDbContext(string connectionString) {
        _connectionString = connectionString;
    }

    public AgendaDbContext(DbContextOptions<AgendaDbContext> options) : base(options) {
        _connectionString = "";
    }

    public DbSet<ContactoEntity> Contactos { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlite(_connectionString);
    }

    public void EnsureCreated() {
        Database.EnsureCreated();
    }
}