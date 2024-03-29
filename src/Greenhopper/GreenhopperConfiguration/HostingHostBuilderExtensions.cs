﻿using Greenhopper.Core.Cache;
using Greenhopper.Core.Services;
using Greenhopper.SettingsConfiguration;
using GSF.CarbonAware.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Greenhopper.HostingHostBuilderExtensions;

/// <summary>
/// Adds support for the carbon aware framework
/// </summary>
public static class HostingHostBuilderExtensions
{
    /// <summary>
    /// Configures Greenhopper
    /// </summary>
    /// <param name="hostBuilder">An instance of a <see cref="IHostBuilder"/>.</param>
    /// <returns> The same instance of the <see cref="IHostBuilder"/> for chaining.</returns>
    public static IHostBuilder ConfigureGreenhopper(this IHostBuilder hostBuilder)
    {

        return hostBuilder
            .ConfigureAppConfiguration(builder =>
            {
                builder.ConfigureSettingDefaults();
            })
            .ConfigureServices(sc =>
            {
                var serviceProvider = sc.BuildServiceProvider();

                var config = serviceProvider.GetService<IConfiguration>();
                sc.AddForecastServices(config!);
                sc.AddLogging(builder => builder.AddDebug());
                sc.AddSingleton<IGreenhopperService, GreenhopperService>();
                sc.AddMemoryCache();
                sc.AddSingleton<ICacheManager, MemoryCacheManager>();
                sc.AddSingleton<IForecastDataCollector, ForecastDataCollector>();
            });
    }
}