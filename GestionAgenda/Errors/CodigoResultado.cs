namespace GestionAgenda.Errors;

/// <summary>
/// Codigos de resultado de las operaciones de la aplicacion.
/// Se han tomado "prestados" los numeros del estandar HTTP (200, 201, 404, 409, 500...)
/// </summary>
public enum CodigoResultado
{
    /// <summary>La operacion se ha completado correctamente.</summary>
    Ok = 200,
 
    /// <summary>Nuevo recurso (por ejemplo, contacto nuevo).</summary>
    Creado = 201,
 
    /// <summary>Los datos de entrada no son validos (validacion de negocio).</summary>
    PeticionInvalida = 400,
 
    /// <summary>Recurso solicitado no existe.</summary>
    NoEncontrado = 404,
 
    /// <summary>Operacion en conflicto.</summary>
    Conflicto = 409,
 
    /// <summary>Error inesperado.</summary>
    ErrorInterno = 500
}