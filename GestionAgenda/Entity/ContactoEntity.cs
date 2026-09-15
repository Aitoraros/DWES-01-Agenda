using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionAgenda.Entity;

/// <summary>
/// Entidad para la Base de datos para Contactos.
/// </summary>
[Table("Contactos")]
public class ContactoEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    public string Alias { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    public string Telefono { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "datetime2")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
}