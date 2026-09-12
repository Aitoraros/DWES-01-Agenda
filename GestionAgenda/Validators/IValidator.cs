namespace GestionAgenda.Validators;

public interface IValidador<in T>
{
    IEnumerable<string> Validar(T entidad);
}