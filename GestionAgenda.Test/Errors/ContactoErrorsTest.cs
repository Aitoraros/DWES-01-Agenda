using FluentAssertions;
using GestionAgenda.Errors;

namespace GestionAgenda.Test.Errors;

[TestFixture]
public class ContactoErrorsTest
{
    [TestFixture]
    public class CasosPositivos
    {
        [TestCase(CodigoResultado.Ok, "La operación se ha completado correctamente.")]
        [TestCase(CodigoResultado.Creado, "Se ha creado el recurso correctamente.")]
        [TestCase(CodigoResultado.PeticionInvalida, "Los datos de entrada no son válidos.")]
        [TestCase(CodigoResultado.NoEncontrado, "El recurso solicitado no existe.")]
        [TestCase(CodigoResultado.Conflicto, "La operación entra en conflicto con el estado actual.")]
        [TestCase(CodigoResultado.ErrorInterno, "Ha ocurrido un error inesperado.")]
        public void MensajePorDefecto_CuandoMensajeEsNull_AsignaMensajeSegunCodigo(CodigoResultado codigo, string mensajeEsperado)
        {
            // Act
            var error = new ContactoErrors(codigo, null);

            // Assert
            error.Codigo.Should().Be(codigo);
            error.Mensaje.Should().Be(mensajeEsperado);
        }

        [Test]
        public void Constructor_CuandoMensajeTieneValor_ConservaMensajePersonalizado()
        {
            // Arrange
            const string mensajePersonalizado = "Error personalizado de validación de teléfono.";

            // Act
            var error = new ContactoErrors(CodigoResultado.PeticionInvalida, mensajePersonalizado);

            // Assert
            error.Codigo.Should().Be(CodigoResultado.PeticionInvalida);
            error.Mensaje.Should().Be(mensajePersonalizado);
        }

        [Test]
        public void ToString_DevuelveFormatoCorrectoConCodigoYMensaje()
        {
            // Arrange
            var error = new ContactoErrors(CodigoResultado.NoEncontrado, null);
            var codigoInt = (int)CodigoResultado.NoEncontrado;

            // Act
            var resultado = error.ToString();

            // Assert
            resultado.Should().Be($"[{codigoInt}] El recurso solicitado no existe.");
        }
    }

    [TestFixture]
    public class CasosInvalidos
    {
        [Test]
        public void MensajePorDefecto_CodigoNoMapeadoEnEnum_DevuelveEstadoDesconocido()
        {
            // Arrange
            var codigoNoExiste = (CodigoResultado)999;

            // Act
            var error = new ContactoErrors(codigoNoExiste, null);

            // Assert
            error.Codigo.Should().Be(codigoNoExiste);
            error.Mensaje.Should().Be("Estado desconocido.");
        }
    }
}