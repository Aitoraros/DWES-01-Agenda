using System.Globalization;
using FluentAssertions;
using GestionAgenda.Config;

namespace GestionAgenda.Test.Config;

[TestFixture]
public class AppConfigTest
{
    [TestFixture]
    public class CasosValidos
    {
        [Test]
        public void Config_DebeEstarInicializadoYNoSerNull()
        {
            // Act
            var config = AppConfig.Config;

            // Assert
            config.Should().NotBeNull("appsettings.json debe ser leído al cargar el AppConfig");
        }

        [Test]
        public void Locale_DebeEstarConfiguradoEnEspanolDeEspana()
        {
            // Act
            CultureInfo locale = AppConfig.Locale;

            // Assert
            locale.Name.Should().Be("es-ES");
        }

        [Test]
        public void ConnectionString_DebeDevolverCadenaNoVacia()
        {
            // Act
            string connString = AppConfig.ConnectionString;

            // Assert
            connString.Should().NotBeNullOrWhiteSpace();
            connString.Should().Contain("Data Source=");
        }

        [Test]
        public void DataFolder_DebeEstarEnElDirectorioBase()
        {
            // Arrange
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Act
            string dataFolder = AppConfig.DataFolder;

            // Assert
            dataFolder.Should().StartWith(baseDir);
        }

        [Test]
        public void CacheSize_DebeDevolverUnValorMayorQueCero()
        {
            // Act
            int cacheSize = AppConfig.CacheSize;

            // Assert
            cacheSize.Should().BeGreaterThan(0);
        }

        [Test]
        public void PropiedadesSerilog_DebenEstarConfiguradasConValoresPorDefectoOConfigurados()
        {
            // Act
            string minLevel = AppConfig.LogMinimumLevel;
            string logPath = AppConfig.LogFilePath;
            int retainedFiles = AppConfig.LogRetainedFiles;
            string template = AppConfig.LogOutputTemplate;

            // Assert
            minLevel.Should().NotBeNullOrWhiteSpace();
            logPath.Should().NotBeNullOrWhiteSpace();
            retainedFiles.Should().BeGreaterThan(0);
            template.Should().NotBeNullOrWhiteSpace();
        }
    }

    [TestFixture]
    public class CasosInvalidos
    {
        [Test]
        public void DataFolder_Y_LogFilePath_NoDebenSerNulosNiEstarVacios()
        {
            // Act
            var rutaDatos = AppConfig.DataFolder;
            var rutaLogs = AppConfig.LogFilePath;

            // Assert
            rutaDatos.Should().NotBeNullOrWhiteSpace();
            rutaLogs.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void LogRetainedFiles_DebeSerMayorOIgualACero()
        {
            // Act
            var limiteArchivos = AppConfig.LogRetainedFiles;

            // Assert
            limiteArchivos.Should().BeGreaterThanOrEqualTo(0);
        }
    }
}