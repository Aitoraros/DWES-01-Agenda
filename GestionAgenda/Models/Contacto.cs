namespace GestionAgenda.Models;

public class Contacto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    
    //metadatos
    
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    
    public override string ToString()
    {
        var aliasTexto = string.IsNullOrWhiteSpace(Alias) ? "Sin alias" : Alias;
        return $"[ID: {Id}] {Nombre} - Tel: {Telefono} | Email: {Email} | Alias: {aliasTexto}";
    }
}