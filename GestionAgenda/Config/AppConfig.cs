using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace GestionAgenda.Config;

public class AppConfig
{
    static AppConfig() {
        Config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }
    
    public static IConfiguration Config { get; }
    public static CultureInfo Locale = CultureInfo.GetCultureInfo("es-ES");
    
    public static string ConnectionString => 
        Config.GetValue<string>("Repository:ConnectionString") ?? "Data Source=data/agenda.db";

    public static int CacheSize => Config.GetValue("Cache:Size", 5);

    public static string DataFolder => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, // portable
        Config.GetValue<string>("Repository:Directory") ?? "data");
    
    public static bool DropData => Config.GetValue<bool>("Repository:DropData", false);

    public static bool SeedData => Config.GetValue<bool>("Repository:SeedData", true);
    
    // logs
    public static string LogMinimumLevel => 
        Config.GetValue<string>("Serilog:MinimumLevel") ?? "Debug";

    public static string LogFilePath => 
        Config.GetValue<string>("Serilog:WriteTo:1:Args:path") ?? "log/log-.txt";

    public static int LogRetainedFiles => 
        Config.GetValue<int>("Serilog:WriteTo:1:Args:retainedFileCountLimit", 5);

    public static string LogOutputTemplate => 
        Config.GetValue<string>("Serilog:WriteTo:1:Args:outputTemplate") ?? 
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";
    
}