using Greenhopper.Core.Cache;
using Greenhopper.Core.Exceptions;
using GSF.CarbonAware.Handlers;
using GSF.CarbonAware.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace Greenhopper.Core.Services;

/// <summary>
/// Service that retrieves emmissions forecasta data using the Carbon Aware SDK 
/// </summary>
public class ForecastDataCollector : IForecastDataCollector
{

    private readonly ILogger<ForecastDataCollector> _logger;

    private readonly IForecastHandler _forecastHandler;

    private static readonly ActivitySource Activity = new(nameof(ForecastDataCollector));

    private readonly ICacheManager _cacheManager;

    /// <summary>
    /// Creates an instance of a <see cref="ForecastDataCollector"/>.
    /// </summary>
    /// <param name="loggerFactory">An instance used to configure the logging system and create <see cref="ILogger"/> instances.</param>
    /// <param name="forecastHandler">A Carbon Aware Forewast Handler.</param>
    /// <param name="cacheManager">A Cache Manager insance.</param>
    public ForecastDataCollector(
        ILoggerFactory loggerFactory,
        IForecastHandler forecastHandler,
        ICacheManager cacheManager
        )
    {
        ExceptionExtensions.ThrowIfNull(loggerFactory);
        ExceptionExtensions.ThrowIfNull(forecastHandler);
        ExceptionExtensions.ThrowIfNull(cacheManager);

        _logger = loggerFactory.CreateLogger<ForecastDataCollector>();
        _forecastHandler = forecastHandler;
        _cacheManager = cacheManager;
    }

    /// <inheritdoc/>
    public async Task<EmissionsForecast> GetAsync(string region, DateTimeOffset datetime, int nextXHoursForAnExecutionWindow, int estimatedExecutionDuration)
    {
        ExceptionExtensions.ThrowIfNullOrWhiteSpace(region);
        ExceptionExtensions.ThrowIfOutsideBounds(datetime, DateTimeOffset.Now, DateTimeOffset.MaxValue);
        ExceptionExtensions.ThrowIfOutsideBounds(nextXHoursForAnExecutionWindow, 1, int.MaxValue);
        ExceptionExtensions.ThrowIfOutsideBounds(estimatedExecutionDuration, 1, nextXHoursForAnExecutionWindow * 60);

        var forecastResult = await _cacheManager.AddOrGetExisting($"{region}-{datetime.Ticks}-{nextXHoursForAnExecutionWindow}-{estimatedExecutionDuration}",
            async () => await _forecastHandler.GetCurrentForecastAsync( 
                new string[] { region }, 
                datetime, 
                datetime.AddHours(nextXHoursForAnExecutionWindow), 
                estimatedExecutionDuration
            ), 5 * 60);//5 minutes

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var stringforeCastData = JsonSerializer.Serialize(forecastResult, new JsonSerializerOptions
            {
                WriteIndented = true
            });
#pragma warning disable CA2254 // Template should be a static expression
            _logger.LogDebug($"Forecast data for '{region}' and {datetime}: {Environment.NewLine} {stringforeCastData}");
#pragma warning restore CA2254 // Template should be a static expression 
        }

        ExceptionExtensions.ThrowIfNullOrEmpty(forecastResult);

        return forecastResult.First();//one region, one result;
    }
}
