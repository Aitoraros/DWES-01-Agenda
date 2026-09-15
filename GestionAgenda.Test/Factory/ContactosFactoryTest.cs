using FluentAssertions;
using GestionAgenda.Factory;

namespace GestionAgenda.Test.Factory;

[TestFixture]
public class ContactosFactoryTest
{
    [TestFixture]
    public class CasosValidos
    {
        [Test]
        public void Seed_DebeDevolverColeccionNoVaciaConListaDeContactos()
        {
            // Act
            var contactos = ContactosFactory.Seed().ToList();

            // Assert
            contactos.Should().NotBeNull();
            contactos.Should().HaveCount(20);
        }

        [Test]
        public void Seed_TodosLosContactosDebenTenerCamposObligatoriosValidos()
        {
            // Act
            var contactos = ContactosFactory.Seed().ToList();

            // Assert
            contactos.Should().OnlyContain(c => c.Id > 0, "todos los contactos deben tener un Id positivo");
            contactos.Should().OnlyContain(c => !string.IsNullOrWhiteSpace(c.Nombre), "todos deben tener nombre");
            contactos.Should().OnlyContain(c => !string.IsNullOrWhiteSpace(c.Email), "todos deben tener email");
            contactos.Should().OnlyContain(c => !string.IsNullOrWhiteSpace(c.Telefono), "todos deben tener teléfono");
        }

        [Test]
        public void Seed_DebeIncluirTantoContactosActivosComoEliminados()
        {
            // Act
            var contactos = ContactosFactory.Seed().ToList();

            // Assert
            contactos.Should().Contain(c => !c.IsDeleted, "deben existir contactos activos");
            contactos.Should().Contain(c => c.IsDeleted, "deben existir contactos marcados como eliminados");
        }

        [Test]
        public void Seed_LasFechasDeCreacionYActualizacionDebenSerCoherentes()
        {
            // Act
            var contactos = ContactosFactory.Seed().ToList();

            // Assert
            contactos.Should().OnlyContain(c => c.UpdatedAt >= c.CreatedAt, 
                "la fecha de actualización nunca puede ser anterior a la de creación");
        }
    }

    [TestFixture]
    public class CasosInvalidos
    {
        [Test]
        public void Seed_NoDebeContenerIdsDuplicados()
        {
            // Act
            var contactos = ContactosFactory.Seed().ToList();
            var idsUnicos = contactos.Select(c => c.Id).Distinct();

            // Assert
            idsUnicos.Count().Should().Be(contactos.Count, "no debe haber dos contactos con el mismo Id en el Seed");
        }

        [Test]
        public void Seed_NoDebeContenerAliasDuplicadosEntreContactosNoVacios()
        {
            // Act
            var contactosConAlias = ContactosFactory.Seed()
                .Where(c => !string.IsNullOrWhiteSpace(c.Alias))
                .Select(c => c.Alias.ToLower())
                .ToList();

            var aliasUnicos = contactosConAlias.Distinct();

            // Assert
            aliasUnicos.Count().Should().Be(contactosConAlias.Count, "los alias definidos deben ser únicos en los datos iniciales");
        }
    }
}