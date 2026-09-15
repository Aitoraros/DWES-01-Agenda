using GestionAgenda.Models;

namespace GestionAgenda.Validators;

    public class ContactoValidator : IValidador<Contacto>
    {
        private readonly ValidatorFunctions _validatorFunctions = new();
        
        public IEnumerable<string> Validar(Contacto contacto)
        {
            var errores = new List<string>();

            if (contacto is null)
            {
                errores.Add("El contacto no puede ser nulo");
                return errores;
            }

            if (!_validatorFunctions.NombreValido(contacto.Nombre))
            {
                errores.Add("El nombre no es válido (debe tener entre 2 y 9 caracteres).");
            }

            if (!_validatorFunctions.AliasValido(contacto.Alias))
            {
                errores.Add("El alias no es válido (debe tener entre 2 y 14 caracteres).");
            }
            
            if (!_validatorFunctions.EmailValido(contacto.Email))
            {
                errores.Add("El correo electrónico no tiene un formato válido.");
            }

            if (!_validatorFunctions.TelefonoValido(contacto.Telefono))
            {
                errores.Add("El número de teléfono no tiene un formato válido.");
            }

            return errores;
        }
    }