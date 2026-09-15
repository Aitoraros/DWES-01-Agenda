namespace GestionAgenda.Errors;

/// <summary>
/// Esta clase devuelve unicamente el codigo de error y un breve mensaje del resultado de la operación
/// </summary>


public class ContactoErrors
{
    public CodigoResultado Codigo { get; }
    public string Mensaje { get; }

    public ContactoErrors(CodigoResultado codigo, string? mensaje)
    {
        Codigo = codigo;
        Mensaje = mensaje ?? MensajePorDefecto(codigo);
    }
    
    public override string ToString() => $"[{(int)Codigo}] {Mensaje}";
    
    private static string MensajePorDefecto(CodigoResultado codigo) => codigo switch
    {
        CodigoResultado.Ok => "La operación se ha completado correctamente.",
        CodigoResultado.Creado => "Se ha creado el recurso correctamente.",
        CodigoResultado.PeticionInvalida => "Los datos de entrada no son válidos.",
        CodigoResultado.NoEncontrado => "El recurso solicitado no existe.",
        CodigoResultado.Conflicto => "La operación entra en conflicto con el estado actual.",
        CodigoResultado.ErrorInterno => "Ha ocurrido un error inesperado.",
        _ => "Estado desconocido."
    };
}