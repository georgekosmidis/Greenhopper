using CarbonAware.Aggregators.Configuration;
using CarbonAware.AzureFunction.Services.SettingsConfiguration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CarbonAware.AzureFunction.Services.HostingHostBuilderExtensions;

/// <summary>
/// Adds support for the carbon aware framework
/// </summary>
public static class HostingHostBuilderExtensions
{

    public static IHostBuilder ConfigureCarbonAwareApp(this IHostBuilder hostBuilder)
    {
        var config = new ConfigurationBuilder()
            .UseCarbonAwareDefaults()
            .Build();

        return hostBuilder
            .ConfigureAppConfiguration(builder =>
            {
                builder.UseCarbonAwareDefaults();
            })
            .ConfigureServices(sc =>
            {
                sc.AddLogging(builder => builder.AddDebug());

                string? errorMessage = "";
                bool successfulEmissionServices = sc.TryAddCarbonAwareEmissionServices(config, out errorMessage);
                var serviceProvider = sc.BuildServiceProvider();

                if (!successfulEmissionServices)
                {
                    var _logger = serviceProvider.GetService<ILogger<IHostBuilder>>();
                    _logger?.LogError(errorMessage);
                }

                sc.AddSingleton<IExecutionWindowCalculatorService, ExecutionWindowCalculatorService>();
            });
    }
}