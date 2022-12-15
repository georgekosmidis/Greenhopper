using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Grasshopper.SettingsConfiguration;

/// <summary>
/// Extensions for the <see cref="IConfigurationBuilder"/>.
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// A constant with the Developer environment variable name.
    /// </summary>
    public const string DevelopmentEnvironment = "Development";
    /// <summary>
    /// A constant with the Production environment variable name.
    /// </summary>
    public const string ProductionEnvironment = "Production";

    /// <summary>
    /// Configures the appsettings order depending on the current environment.
    /// </summary>
    /// <param name="builder">The instance of type <see cref="IConfigurationBuilder"/> used to build application configuration.</param>
    /// <returns>The same <see cref="IConfigurationBuilder"/> for chaining.</returns>
    public static IConfigurationBuilder ConfigureSettingDefaults(this IConfigurationBuilder builder)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? ProductionEnvironment;

        builder.AddJsonFile("appsettings.json", optional: true);
        if (env.Equals(DevelopmentEnvironment, StringComparison.OrdinalIgnoreCase))
        {
            builder.AddUserSecrets(Assembly.GetEntryAssembly(), true);
        }
        builder.AddEnvironmentVariables();

        return builder;
    }
}