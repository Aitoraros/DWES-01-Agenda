using FluentAssertions;
using GestionAgenda.Entity;
using GestionAgenda.Models;
using GestionAgenda.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GestionAgenda.Test.Repositories;

[TestFixture]
public class ContactoRepositoryEfCoreTest
{
    private SqliteConnection _connection = null!;
    private AgendaDbContext _context = null!;
    private ContactoRepositoryEfCore _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AgendaDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AgendaDbContext(options);
        _context.Database.EnsureCreated();
        _repository = new ContactoRepositoryEfCore(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    [TestFixture]
    public class CasosValidos : ContactoRepositoryEfCoreTest
    {
        [Test]
        public void Create_ConContactoValido_GuardaEnBaseDeDatosYDevuelveContacto()
        {
            // Arrange
            var contacto = new Contacto 
            { 
                Nombre = "Carlos", 
                Alias = "Charli", 
                Email = "carlos@test.com", 
                Telefono = "612345678" 
            };

            // Act
            var resultado = _repository.Create(contacto);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Alias.Should().Be("Charli");
            resultado.Value.Id.Should().BeGreaterThan(0);
            _context.Contactos.Count().Should().Be(1);
        }

        [Test]
        public void GetById_CuandoExisteYNoEstaBorrado_DevuelveContacto()
        {
            // Arrange
            var entity = new ContactoEntity 
            { 
                Nombre = "Carlos", 
                Alias = "Charli", 
                Email = "carlos@test.com", 
                Telefono = "612345678", 
                IsDeleted = false 
            };
            _context.Contactos.Add(entity);
            _context.SaveChanges();

            // Act
            var resultado = _repository.GetById(entity.Id);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Id.Should().Be(entity.Id);
        }

        [Test]
        public void GetByAlias_CuandoExisteSinImportarMayusculas_DevuelveContacto()
        {
            // Arrange
            var entity = new ContactoEntity 
            { 
                Nombre = "Carlos", 
                Alias = "Charli", 
                Email = "carlos@test.com", 
                Telefono = "612345678", 
                IsDeleted = false 
            };
            _context.Contactos.Add(entity);
            _context.SaveChanges();

            // Act
            var resultado = _repository.GetByAlias("CHARLI");

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Alias.Should().Be("Charli");
        }

        [TestCase("Carlos", 1, 10, 1)]
        [TestCase(null, 1, 10, 2)]
        [TestCase("", 1, 10, 2)]
        public void GetAll_AplicaFiltroYPaginacionCorrectamente(string? texto, int pagina, int tamano, int esperados)
        {
            // Arrange
            _context.Contactos.AddRange(
                new ContactoEntity { Nombre = "Carlos", Alias = "Charli", Email = "carlos@test.com", Telefono = "612345678", IsDeleted = false },
                new ContactoEntity { Nombre = "Ana", Alias = "Anita", Email = "ana@test.com", Telefono = "699999999", IsDeleted = false },
                new ContactoEntity { Nombre = "Borrado", Alias = "Ghost", Email = "ghost@test.com", Telefono = "600000000", IsDeleted = true }
            );
            _context.SaveChanges();

            // Act
            var resultado = _repository.GetAll(texto, pagina, tamano);

            // Assert
            resultado.Should().HaveCount(esperados);
        }

        [Test]
        public void Update_ConContactoExistente_ActualizaCamposEnBaseDeDatos()
        {
            // Arrange
            var entity = new ContactoEntity 
            { 
                Nombre = "Carlos", 
                Alias = "Charli", 
                Email = "carlos@test.com", 
                Telefono = "612345678", 
                IsDeleted = false 
            };
            _context.Contactos.Add(entity);
            _context.SaveChanges();

            var contactoModificado = new Contacto 
            { 
                Id = entity.Id, 
                Nombre = "Carlos Alberto", 
                Alias = "Charli", 
                Email = "carlos@test.com", 
                Telefono = "612345678" 
            };

            // Act
            var resultado = _repository.Update(contactoModificado);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            resultado.Value.Nombre.Should().Be("Carlos Alberto");
        }

        [Test]
        public void Delete_ConIdExistente_AplicaSoftDelete()
        {
            // Arrange
            var entity = new ContactoEntity 
            { 
                Nombre = "Carlos", 
                Alias = "Charli", 
                Email = "carlos@test.com", 
                Telefono = "612345678", 
                IsDeleted = false 
            };
            _context.Contactos.Add(entity);
            _context.SaveChanges();

            // Act
            var resultado = _repository.Delete(entity.Id);

            // Assert
            resultado.IsSuccess.Should().BeTrue();
            _context.Contactos.First(c => c.Id == entity.Id).IsDeleted.Should().BeTrue();
        }
    }

    [TestFixture]
    public class CasosInvalidos : ContactoRepositoryEfCoreTest
    {
        [Test]
        public void Create_ConAliasDuplicado_DevuelveErrorDeConflicto()
        {
            // Arrange
            _context.Contactos.Add(new ContactoEntity 
            { 
                Nombre = "Carlos", 
                Alias = "Charli", 
                Email = "carlos@test.com", 
                Telefono = "612345678", 
                IsDeleted = false 
            });
            _context.SaveChanges();

            var nuevoContacto = new Contacto 
            { 
                Nombre = "Otro", 
                Alias = "CHARLI", 
                Email = "otro@test.com", 
                Telefono = "699999999" 
            };

            // Act
            var resultado = _repository.Create(nuevoContacto);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().Contain("Ya existe un contacto con el alias");
        }

        [Test]
        public void GetById_CuandoNoExisteOEstaBorrado_DevuelveErrorNoEncontrado()
        {
            // Arrange
            var entity = new ContactoEntity 
            { 
                Nombre = "Borrado", 
                Alias = "Ghost", 
                Email = "ghost@test.com", 
                Telefono = "600000000", 
                IsDeleted = true 
            };
            _context.Contactos.Add(entity);
            _context.SaveChanges();

            // Act
            var resultado = _repository.GetById(entity.Id);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().Contain($"No existe ningun contacto con Id={entity.Id}");
        }

        [Test]
        public void GetByAlias_CuandoNoExiste_DevuelveErrorNoEncontrado()
        {
            // Act
            var resultado = _repository.GetByAlias("Inexistente");

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().Contain("No existe ningun contacto con alias 'Inexistente'");
        }

        [Test]
        public void Update_CuandoContactoNoExiste_DevuelveErrorNoEncontrado()
        {
            // Arrange
            var contacto = new Contacto 
            { 
                Id = 999, 
                Nombre = "Carlos", 
                Alias = "Charli", 
                Email = "carlos@test.com", 
                Telefono = "612345678" 
            };

            // Act
            var resultado = _repository.Update(contacto);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().Contain("No existe ningun contacto con Id=999");
        }

        [Test]
        public void Update_ConAliasDuplicadoDeOtroContacto_DevuelveErrorDeConflicto()
        {
            // Arrange
            var c1 = new ContactoEntity { Nombre = "Carlos", Alias = "Charli", Email = "carlos@test.com", Telefono = "612345678", IsDeleted = false };
            var c2 = new ContactoEntity { Nombre = "Ana", Alias = "Anita", Email = "ana@test.com", Telefono = "699999999", IsDeleted = false };
            
            _context.Contactos.AddRange(c1, c2);
            _context.SaveChanges();

            var contactoModificado = new Contacto 
            { 
                Id = c2.Id, 
                Nombre = "Ana", 
                Alias = "Charli", 
                Email = "ana@test.com", 
                Telefono = "699999999" 
            };

            // Act
            var resultado = _repository.Update(contactoModificado);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().Contain("Ya existe otro contacto con el alias 'Charli'");
        }

        [Test]
        public void Delete_CuandoContactoNoExisteOYaEstaBorrado_DevuelveErrorNoEncontrado()
        {
            // Act
            var resultado = _repository.Delete(999);

            // Assert
            resultado.IsFailure.Should().BeTrue();
            resultado.Error.Should().Contain("No existe ningun contacto con Id=999");
        }
    }
}