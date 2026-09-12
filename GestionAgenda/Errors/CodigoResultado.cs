namespace GestionAgenda.Errors;

/// <summary>
/// Codigos de resultado de las operaciones de la aplicacion.
///
/// Esto NO es una API REST. Se han tomado "prestados" los
/// numeros del estandar HTTP (200, 201, 404, 409, 500...)
/// </summary>
public enum CodigoResultado
{
    /// <summary>La operacion se ha completado correctamente.</summary>
    Ok = 200,
 
    /// <summary>Se ha creado un nuevo recurso (por ejemplo, un contacto nuevo).</summary>
    Creado = 201,
 
    /// <summary>Los datos de entrada no son validos (validacion de negocio).</summary>
    PeticionInvalida = 400,
 
    /// <summary>El recurso solicitado (contacto por Id o por alias) no existe.</summary>
    NoEncontrado = 404,
 
    /// <summary>La operacion entra en conflicto con el estado actual (p. ej. alias duplicado).</summary>
    Conflicto = 409,
 
    /// <summary>Ha ocurrido un error inesperado (excepcion de infraestructura, BD, etc.).</summary>
    ErrorInterno = 500
}