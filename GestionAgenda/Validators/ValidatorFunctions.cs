using System.Text.RegularExpressions;

namespace GestionAgenda.Validators;

public class ValidatorFunctions
{
    private const string TelefonoRegex = @"^\+?\d{9,15}$";
    private const string EmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    
    public bool NombreValido(string nombre) =>
        !string.IsNullOrEmpty(nombre) && nombre.Trim().Length < 10 && nombre.Trim().Length > 1;

    public bool AliasValido(string alias) =>
        !string.IsNullOrEmpty(alias) && alias.Trim().Length < 15 && alias.Trim().Length > 1;

    public bool EmailValido(string email) =>
        Regex.IsMatch(email.Trim(), EmailRegex);

    public bool TelefonoValido(string telefono) =>
        Regex.IsMatch(telefono.Trim(), TelefonoRegex);
}

