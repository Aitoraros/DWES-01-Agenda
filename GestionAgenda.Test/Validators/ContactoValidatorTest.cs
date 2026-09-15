using FluentAssertions;
using GestionAgenda.Models;
using GestionAgenda.Validators;

namespace GestionAgenda.Test.Validators;

[TestFixture]
public class ContactoValidatorTest
{
    [TestFixture]
    public class ValidacionContacto
    {
        private ContactoValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new ContactoValidator();
        }

        [TestFixture]
        public class CasosValidos : ValidacionContacto
        {
            [Test]
            public void Validar_DebeDevolverListaVacia_CuandoContactoEsValido()
            {
                // Arrange
                var contactoValido = new Contacto
                {
                    Nombre = "Carlos",
                    Alias = "Charli",
                    Email = "carlos@example.com",
                    Telefono = "612345678"
                };

                // Act
                var errores = _validator.Validar(contactoValido);

                // Assert
                errores.Should().BeEmpty("un contacto con todos sus datos válidos no debe generar errores");
            }
        }

        [TestFixture]
        public class CasosInvalidos : ValidacionContacto
        {
            [Test]
            public void Validar_DebeDevolverError_CuandoContactoEsNull()
            {
                // Arrange
                Contacto contactoNull = null;

                // Act
                var errores = _validator.Validar(contactoNull);

                // Assert
                errores.Should().ContainSingle()
                       .Which.Should().Be("El contacto no puede ser nulo");
            }

            [Test]
            public void Validar_DebeAcumularTodosLosErrores_CuandoTodosLosCamposSonInvalidos()
            {
                // Arrange
                var contactoInvalido = new Contacto
                {
                    Nombre = "N",               
                    Alias = "A",                
                    Email = "correo-invalido",  
                    Telefono = "123"            
                };

                // Act
                var errores = _validator.Validar(contactoInvalido);

                // Assert
                errores.Should().HaveCount(4, "se debe registrar un mensaje de error por cada uno de los 4 campos fallidos");
                errores.Should().Contain(e => e.Contains("nombre"));
                errores.Should().Contain(e => e.Contains("alias"));
                errores.Should().Contain(e => e.Contains("correo electrónico"));
                errores.Should().Contain(e => e.Contains("teléfono"));
            }
        }
    }

    [TestFixture]
    public class FuncionesAuxiliares
    {
        private ValidatorFunctions _functions;

        [SetUp]
        public void SetUp()
        {
            _functions = new ValidatorFunctions();
        }

        [TestFixture]
        public class CasosValidos : FuncionesAuxiliares
        {
            [Test]
            [TestCase("Ana")]        
            [TestCase("Lu")]         
            [TestCase("Alejandro")]  
            [TestCase("  Pedro  ")]  
            public void NombreValido_DebeDevolverTrue_CuandoNombreCumpleLongitud(string nombreValido)
            {
                // Arrange & Act
                bool esValido = _functions.NombreValido(nombreValido);

                // Assert
                esValido.Should().BeTrue($"el nombre '{nombreValido}' tiene entre 2 y 9 caracteres tras aplicar Trim()");
            }

            [Test]
            [TestCase("SuperDev1234")] 
            [TestCase("   DevMaster   ")] 
            public void AliasValido_DebeDevolverTrue_CuandoAliasCumpleLongitud(string aliasValido)
            {
                // Arrange & Act
                bool esValido = _functions.AliasValido(aliasValido);

                // Assert
                esValido.Should().BeTrue($"el alias '{aliasValido}' tiene entre 2 y 14 caracteres tras aplicar Trim()");
            }

            [Test]
            [TestCase("usuario@dominio.com")]
            [TestCase("juan.perez@empresa.es")]
            [TestCase("   test@test.org   ")]
            public void EmailValido_DebeDevolverTrue_CuandoFormatoEsCorrecto(string emailValido)
            {
                // Arrange & Act
                bool esValido = _functions.EmailValido(emailValido);

                // Assert
                esValido.Should().BeTrue($"el email '{emailValido}' cumple con el patrón de correo");
            }

            [Test]
            [TestCase("612345678")]        
            [TestCase("+34612345678")]     
            [TestCase("123456789012345")]   
            public void TelefonoValido_DebeDevolverTrue_CuandoFormatoEsCorrecto(string telefonoValido)
            {
                // Arrange & Act
                bool esValido = _functions.TelefonoValido(telefonoValido);

                // Assert
                esValido.Should().BeTrue($"el teléfono '{telefonoValido}' tiene entre 9 y 15 dígitos");
            }
        }

        [TestFixture]
        public class CasosInvalidos : FuncionesAuxiliares
        {
            [Test]
            [TestCase("")]
            [TestCase("   ")]
            [TestCase("A")]                     
            [TestCase("JuanCarlos")]            
            [TestCase("NombreDemasiadoLargo")] 
            public void NombreValido_DebeDevolverFalse_CuandoNombreEsInvalido(string nombreInvalido)
            {
                // Arrange & Act
                bool esValido = _functions.NombreValido(nombreInvalido);

                // Assert
                esValido.Should().BeFalse($"el nombre '{nombreInvalido}' no respeta la longitud válida (2-9)");
            }

            [Test]
            [TestCase("")]
            [TestCase("   ")]
            [TestCase("A")]                     
            [TestCase("AliasDemasiadoLargo1")]    
            public void AliasValido_DebeDevolverFalse_CuandoAliasEsInvalido(string aliasInvalido)
            {
                // Arrange & Act
                bool esValido = _functions.AliasValido(aliasInvalido);

                // Assert
                esValido.Should().BeFalse($"el alias '{aliasInvalido}' no respeta la longitud válida (2-14)");
            }

            [Test]
            [TestCase("")]
            [TestCase("sinArroba.com")]
            [TestCase("usuario@sinDominio")]
            [TestCase("@dominio.com")]
            public void EmailValido_DebeDevolverFalse_CuandoFormatoEsIncorrecto(string emailInvalido)
            {
                // Arrange & Act
                bool esValido = _functions.EmailValido(emailInvalido);

                // Assert
                esValido.Should().BeFalse($"el email '{emailInvalido}' no cumple el patrón usuario@dominio.ext");
            }

            [Test]
            [TestCase("")]
            [TestCase("12345678")]          
            [TestCase("1234567890123456")]  
            [TestCase("61234567A")]         
            [TestCase("++34612345678")]     
            public void TelefonoValido_DebeDevolverFalse_CuandoFormatoEsIncorrecto(string telefonoInvalido)
            {
                // Arrange & Act
                bool esValido = _functions.TelefonoValido(telefonoInvalido);

                // Assert
                esValido.Should().BeFalse($"el teléfono '{telefonoInvalido}' no cumple el formato telefónico");
            }
        }
    }
}