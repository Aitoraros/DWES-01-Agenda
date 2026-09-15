using FluentAssertions;
using GestionAgenda.Cache;

namespace GestionAgenda.Test.Cache;

public class LruCacheTest
{
    [TestFixture]
    public class CasosPositivos
    {
        private LruCache<int, string> _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new LruCache<int, string>(3);
        }
        
        [Test]
        public void Add_ElementoValido_GuardaElemento() {

            // Act
            _cache.Add(1, "uno");

            // Assert
            _cache.Get(1).Should().Be("uno");
        }

        [Test]
        public void Add_MuchosElementos_GuardaTodos() {

            // Act
            _cache.Add(1, "uno");
            _cache.Add(2, "dos");
            _cache.Add(3, "tres");

            // Assert
            _cache.Get(1).Should().Be("uno");
            _cache.Get(2).Should().Be("dos");
            _cache.Get(3).Should().Be("tres");
        }

        [Test]
        public void Add_CapacidadSuperada_EliminaMasAntiguo() {

            // Act
            _cache.Add(1, "uno");
            _cache.Add(2, "dos");
            _cache.Add(3, "tres");
            _cache.Add(4, "cuatro");

            // Assert
            _cache.Get(1).Should().BeNull();
            _cache.Get(2).Should().Be("dos");
            _cache.Get(3).Should().Be("tres");
            _cache.Get(4).Should().Be("cuatro");
        }
        
        [Test]
        public void Add_ConClave_ReemplazaValor() {

            // Act
            _cache.Add(1, "uno");
            _cache.Add(1, "UNOReemplazo");

            // Assert
            _cache.Get(1).Should().Be("UNOReemplazo");
        }

        [Test]
        public void Get_SiExiste_ActualizaOrden() {

            // Act
            _cache.Add(7, "siete");
            _cache.Add(8, "ocho");
            _cache.Add(9, "nueve");
            _cache.Get(8);

            // Assert
            _cache.Get(8).Should().Be("ocho");
            _cache.Get(7).Should().Be("siete");
        }
        
        [Test]
        public void Get_SiNoExiste_DevuelveNull() {

            // Act
            var resultado = _cache.Get(1);

            // Assert
            resultado.Should().BeNull();
        }

        [Test]
        public void Remove_SiExiste_EliminaElemento() {

            // Act
            _cache.Add(1, "uno");
            _cache.Add(2, "dos");
            var resultado = _cache.Remove(1);

            // Assert
            resultado.Should().BeTrue();
            _cache.Get(1).Should().BeNull();
            _cache.Get(2).Should().Be("dos");
        }

        [Test]
        public void Remove_SiNoExiste_DevuelveFalse() {

            // Act
            var resultado = _cache.Remove(999);

            // Assert
            resultado.Should().BeFalse();
        }
    }

    [TestFixture]
    public class CasosNegativos {

        [Test]
        public void Constructor_CapacidadCero_LanzaExcepcion() {
            // Arrange y Act
            var action = () => new LruCache<int, string>(0);

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}