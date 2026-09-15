using CSharpFunctionalExtensions;
using FluentAssertions;
using GestionAgenda.Cache;
using GestionAgenda.Models;
using GestionAgenda.Repositories;
using GestionAgenda.Services;
using GestionAgenda.Validators;
using Moq;

namespace GestionAgenda.Test.Services;

[TestFixture]
public class ContactoServiceTest
{
    private Mock<IContactoRepository> _repositoryMock;
    private Mock<IValidador<Contacto>> _validadorMock;
    private Mock<ICache<int, Contacto>> _cacheMock;
    private ContactoService _service;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IContactoRepository>();
        _validadorMock = new Mock<IValidador<Contacto>>();
        _cacheMock = new Mock<ICache<int, Contacto>>();

        _service = new ContactoService(
            _repositoryMock.Object,
            _validadorMock.Object,
            _cacheMock.Object
        );
    }

    [TestFixture]
    public class CasosValidos : ContactoServiceTest
    {
        [Test]
        public void GetById_CuandoEstaEnCache_DevuelveDesdeCacheSinTocarRepositorio()
        {
            // Arrange
            var contacto = new Contacto { Id = 1, Nombre = "Carlos" };
            _cacheMock.Setup(c => c.Get(1)).Returns(contacto);

            // Act
            var resultado = _service.GetById(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Be(contacto);
            _repositoryMock.Verify(r => r.GetById(1), Times.Never);
        }

        [Test]
        public void GetById_CuandoNoEstaEnCache_DevuelveDesdeRepositorioYGuardaEnCache()
        {
            // Arrange
            var contacto = new Contacto { Id = 1, Nombre = "Carlos" };
            _cacheMock.Setup(c => c.Get(1)).Returns((Contacto?)null);
            _repositoryMock.Setup(r => r.GetById(1)).Returns(Result.Success(contacto));

            // Act
            var resultado = _service.GetById(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Be(contacto);
            _cacheMock.Verify(c => c.Add(1, contacto), Times.Once);
        }

        [Test]
        public void GetByAlias_ConAliasValido_DevuelveContacto()
        {
            // Arrange
            var contacto = new Contacto { Id = 1, Alias = "Charlie" };
            _repositoryMock.Setup(r => r.GetByAlias("Charlie")).Returns(Result.Success(contacto));

            // Act
            var resultado = _service.GetByAlias("Charlie");

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().Be(contacto);
        }

        [Test]
        public void GetAll_ConPaginacionValida_DevuelveListaDeContactos()
        {
            // Arrange
            IEnumerable<Contacto> contactos = new List<Contacto> { new Contacto { Id = 1, Nombre = "Carlos" } };
            _repositoryMock.Setup(r => r.GetAll("Carlos", 1, 10)).Returns(contactos);

            // Act
            var resultado = _service.GetAll("Carlos", 1, 10);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Should().BeEquivalentTo(contactos);
        }

        [Test]
        public void CreateContacto_ConDatosValidos_CreaContactoConTrim()
        {
            // Arrange
            _validadorMock.Setup(v => v.Validar(It.Is<Contacto>(c =>
                c.Nombre == "Carlos" &&
                c.Telefono == "612345678" &&
                c.Email == "carlos@test.com" &&
                c.Alias == "Charliee"
            ))).Returns([]);

            _repositoryMock.Setup(r => r.Create(It.Is<Contacto>(c =>
                c.Nombre == "Carlos" &&
                c.Telefono == "612345678" &&
                c.Email == "carlos@test.com" &&
                c.Alias == "Charlie"
            ))).Returns((Contacto c) => Result.Success(c));

            // Act
            var resultado = _service.CreateContacto(" Carlos ", " 612345678 ", " carlos@test.com ", " Charlie ");

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.Create(It.IsAny<Contacto>()), Times.Once);
        }

        [Test]
        public void UpdateContacto_ConIdYCitaValida_ActualizaYLimpiaCache()
        {
            // Arrange
            _validadorMock.Setup(v => v.Validar(It.Is<Contacto>(c => c.Id == 1))).Returns([]);
            _repositoryMock.Setup(r => r.Update(It.Is<Contacto>(c => c.Id == 1)))
                           .Returns((Contacto c) => Result.Success(c));

            // Act
            var resultado = _service.UpdateContacto(1, "Carlos", "612345678", "carlos@test.com", "Charlie");

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _cacheMock.Verify(c => c.Remove(1), Times.Once);
        }

        [Test]
        public void DeleteContacto_ConIdValido_EliminaYLimpiaCache()
        {
            // Arrange
            _repositoryMock.Setup(r => r.Delete(1)).Returns(Result.Success());

            // Act
            var resultado = _service.DeleteContacto(1);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _cacheMock.Verify(c => c.Remove(1), Times.Once);
        }
    }

    [TestFixture]
    public class CasosInvalidos : ContactoServiceTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void GetById_ConIdInvalido_DevuelveErrorSinConsultarCacheNiRepositorio(int idInvalido)
        {
            // Act
            var resultado = _service.GetById(idInvalido);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().Contain("El Id debe ser mayor que 0.");
            _cacheMock.Verify(c => c.Get(idInvalido), Times.Never);
            _repositoryMock.Verify(r => r.GetById(idInvalido), Times.Never);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void GetByAlias_ConAliasVacio_DevuelveError(string? aliasInvalido)
        {
            // Act
            var resultado = _service.GetByAlias(aliasInvalido!);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().Contain("El alias no puede estar vacio.");
            _repositoryMock.Verify(r => r.GetByAlias(It.IsAny<string>()), Times.Never);
        }

        [Test]
        [TestCase(0, 10)]
        [TestCase(1, 0)]
        public void GetAll_ConPaginacionInvalida_DevuelveError(int pagina, int tamano)
        {
            // Act
            var resultado = _service.GetAll(null, pagina, tamano);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().Contain("Pagina y tamanoPagina deben ser mayores que 0.");
            _repositoryMock.Verify(r => r.GetAll(null, pagina, tamano), Times.Never);
        }

        [Test]
        public void CreateContacto_ConErroresDeValidacion_DevuelveErrorSinCrear()
        {
            // Arrange
            _validadorMock.Setup(v => v.Validar(It.IsAny<Contacto>()))
                          .Returns(["[400] El nombre no es válido. El email no es válido."]);

            // Act
            var resultado = _service.CreateContacto("N", "123", "bad-email", "A");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().Be("[400] [400] El nombre no es válido. El email no es válido.");
            _repositoryMock.Verify(r => r.Create(It.IsAny<Contacto>()), Times.Never);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void UpdateContacto_ConIdInvalido_DevuelveError(int idInvalido)
        {
            // Act
            var resultado = _service.UpdateContacto(idInvalido, "Carlos", "612345678", "carlos@test.com", "Charlie");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().Contain("El Id debe ser mayor que 0.");
            _repositoryMock.Verify(r => r.Update(It.IsAny<Contacto>()), Times.Never);
        }

        [Test]
        public void UpdateContacto_ConErroresDeValidacion_DevuelveErrorSinActualizarNiLimpiarCache()
        {
            // Arrange
            _validadorMock.Setup(v => v.Validar(It.IsAny<Contacto>()))
                          .Returns(["El teléfono no es válido."]);

            // Act
            var resultado = _service.UpdateContacto(1, "Carlos", "123", "carlos@test.com", "Charlie");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().Be("[400] El teléfono no es válido.");
            _repositoryMock.Verify(r => r.Update(It.IsAny<Contacto>()), Times.Never);
            _cacheMock.Verify(c => c.Remove(1), Times.Never);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void DeleteContacto_ConIdInvalido_DevuelveError(int idInvalido)
        {
            // Act
            var resultado = _service.DeleteContacto(idInvalido);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().Contain("El Id debe ser mayor que 0.");
            _repositoryMock.Verify(r => r.Delete(idInvalido), Times.Never);
            _cacheMock.Verify(c => c.Remove(idInvalido), Times.Never);
        }
    }
}