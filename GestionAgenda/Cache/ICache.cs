namespace GestionAgenda.Cache;

/// <summary>
/// Contrato para Cache de tipo LRU.
/// </summary>
/// <typeparam name="TKey">Tipo de clave.</typeparam>
/// <typeparam name="TValue">Tipo de valor.</typeparam>
public interface ICache<in TKey, TValue> where TKey : notnull {
    /// <summary>
    /// Agrega el elemento a la caché, eliminando el menos usado si ésta está llena.
    /// </summary>
    /// <param name="key">Clave del elemento</param>
    /// <param name="value">Valor del elemento</param>
    void Add(TKey key, TValue value);
    /// <summary>
    /// Obtiene el elemento de la cache.
    /// </summary>
    /// <param name="key">Clave del elemento</param>
    /// <returns>El valor o en su defecto null.</returns>
    TValue? Get(TKey key);
    /// <summary>
    /// Elimina el elemento de la caché.
    /// </summary>
    /// <param name="key">Clave del elemento</param>
    /// <returns>True si elimina o false si no existe.</returns>
    bool Remove(TKey key);
}