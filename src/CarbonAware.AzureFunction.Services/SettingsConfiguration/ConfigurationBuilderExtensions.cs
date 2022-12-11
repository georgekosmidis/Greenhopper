using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace CarbonAware.AzureFunction.Services.SettingsConfiguration;

public static class ConfigurationBuilderExtensions
{
    public const string DevelopmentEnvironment = "Development";
    public const string ProductionEnvironment = "Production";
    public static IConfigurationBuilder UseCarbonAwareDefaults(this IConfigurationBuilder builder)
    {
        string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? ProductionEnvironment;

        builder.AddJsonFile("appsettings.json", optional: true);
        if (env.Equals(DevelopmentEnvironment, StringComparison.OrdinalIgnoreCase))
        {
            builder.AddUserSecrets(Assembly.GetEntryAssembly(), true);
        }
        builder.AddEnvironmentVariables();

        return builder;
    }
}