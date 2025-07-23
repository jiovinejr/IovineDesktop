using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO;

public static class ConfigurationHelper
{
    private static readonly IConfiguration _configuration;

    static ConfigurationHelper()
    {
        try
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Configuration error: {ex}");
            throw;
        }
    }

    public static string GetConnectionString(string name)
    {
        var connectionString = _configuration.GetConnectionString(name);
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"The connection string '{name}' has not been initialized.");
        }
        return connectionString;
    }
}