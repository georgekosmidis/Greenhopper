using CarbonAware.Exceptions;
using CarbonAware.Model;
using Grasshopper.Core.Exceptions;
using Grasshopper.Core.Services;
using Grasshopper.Models;
using Grasshopper.SettingsConfiguration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Grasshopper;

/// <summary>
/// Service that finds the optimal window for the execution time suggested.
/// </summary>
public class OptimalWindowCalculatorService : IOptimalWindowCalculatorService
{
    private readonly ILogger<OptimalWindowCalculatorService> _logger;

    private readonly IForecastDataCollector _forecastDataCollector;

    private static readonly ActivitySource Activity = new(nameof(OptimalWindowCalculatorService));

    private readonly GrasshoperSettings _settings;

    /// <summary>
    /// Creates an instance of a <see cref="OptimalWindowCalculatorService"/>.
    /// </summary>
    /// <param name="loggerFactory">An instance used to configure the logging system and create <see cref="ILogger"/> instances.</param>
    /// <param name="forecastDataCollector">An instance of the service that can retrieve forecast data from Carbon Aware SDK.</param>
    /// <param name="configuration">The key/value application configuration.</param>
    public OptimalWindowCalculatorService(ILoggerFactory loggerFactory,
        IForecastDataCollector forecastDataCollector,
        IConfiguration configuration)
    {
        ExceptionExtensions.ThrowIfNull(loggerFactory);
        ExceptionExtensions.ThrowIfNull(forecastDataCollector);
        ExceptionExtensions.ThrowIfNull(configuration);

        _logger = loggerFactory.CreateLogger<OptimalWindowCalculatorService>();
        _forecastDataCollector = forecastDataCollector;
        _settings = configuration
                    .GetSection(GrasshoperSettings.Key)
                    .Get<GrasshoperSettings>();
    }

    /// <inheritdoc/>
    public async Task<OptimalWindowResponse> GetForecastDataAsync(string region, DateTimeOffset datetime, int nextXHoursForAnExecutionWindow, int estimatedExecutionDuration)
    {
        using var activity = Activity.StartActivity();

        ExceptionExtensions.ThrowIfNullOrWhiteSpace(region);
        ExceptionExtensions.ThrowIfOutsideBounds(nextXHoursForAnExecutionWindow, 1, int.MaxValue);
        ExceptionExtensions.ThrowIfOutsideBounds(estimatedExecutionDuration, 1, nextXHoursForAnExecutionWindow * 60);
        ExceptionExtensions.ThrowIfOutsideBounds(datetime, DateTimeOffset.Now, DateTimeOffset.MaxValue);

        datetime = RoundUp(datetime, TimeSpan.FromMinutes(5));

        _logger.LogInformation("Requesting forecast for region '{region}' and datetime '{datetime}'", region, datetime);
        var forecastData = await _forecastDataCollector.GetAsync(region, datetime, nextXHoursForAnExecutionWindow, estimatedExecutionDuration);

        //no data
        if (forecastData == default)
        {
            _logger.LogError("No forecast data returned for region '{region}' and datetime '{datetimeNow}'.", region, datetime);
            return !_settings.OnNoForecastExecute
                ? throw new CarbonAwareException($"No forecast returned for region '{region}' and datetime '{datetime}' and '{nameof(GrasshoperSettings.OnNoForecastExecute)}' is false.")
                : new OptimalWindowResponse()
                {
                    IsOptimalWindowNow = false,
                    Data = new EmissionsForecast(),
                    OptimalWindow = DateTime.MinValue
                };
        }

        //no optimal window
        if (!forecastData.OptimalDataPoints.Any())
        {
            _logger.LogError("No optimal execution window in sight, check {config}.", nameof(GrasshoperSettings));
            return new OptimalWindowResponse()
            {
                IsOptimalWindowNow = false,
                Data = forecastData,
                OptimalWindow = DateTime.MaxValue
            };
        }

        //sweet spot?
        if (forecastData.OptimalDataPoints.Any(x => x.Time == datetime))
        {
            _logger.LogInformation("Currently in optimal window; The world is greener thanks to you :)");
            return new OptimalWindowResponse()
            {
                IsOptimalWindowNow = true,
                Data = forecastData,
                OptimalWindow = forecastData.OptimalDataPoints.First().Time
            };
        }

        _logger.LogInformation("Execution skipped; Next probable window of execution is at {time}.", forecastData.OptimalDataPoints.First().Time);
        return new OptimalWindowResponse()
        {
            IsOptimalWindowNow = false,
            Data = forecastData,
            OptimalWindow = forecastData.OptimalDataPoints.First().Time
        };
    }

    /// <inheritdoc/>
    public async Task<bool> IsOptimalWindowNowAsync()
    {
        using var activity = Activity.StartActivity();

        ExceptionExtensions.ThrowIfOutsideBounds(_settings.NextXHoursForAnExecutionWindow, 1, int.MaxValue);
        ExceptionExtensions.ThrowIfOutsideBounds(_settings.EstimatedExecutionDuration, 1, _settings.NextXHoursForAnExecutionWindow * 60);

        var region = Environment.GetEnvironmentVariable(GrasshoperSettings.REGION_NAME)?.Replace(" ", string.Empty).ToLower();
        ExceptionExtensions.ThrowIfNullOrWhiteSpace(region);

        var datetimeNow = RoundUp(DateTimeOffset.Now, TimeSpan.FromMinutes(5));

        var result = await GetForecastDataAsync(region, datetimeNow, _settings.NextXHoursForAnExecutionWindow, _settings.EstimatedExecutionDuration);

        return result.IsOptimalWindowNow;
    }

    /// <inheritdoc/>
    public async Task<bool> IsOptimalWindowNowAsync(int nextXHoursForAnExecutionWindow, int estimatedExecutionDuration)
    {
        using var activity = Activity.StartActivity();

        ExceptionExtensions.ThrowIfOutsideBounds(nextXHoursForAnExecutionWindow, 1, int.MaxValue);
        ExceptionExtensions.ThrowIfOutsideBounds(estimatedExecutionDuration, 1, nextXHoursForAnExecutionWindow * 60);

        var region = Environment.GetEnvironmentVariable(GrasshoperSettings.REGION_NAME)?.Replace(" ", string.Empty).ToLower();
        ExceptionExtensions.ThrowIfNullOrWhiteSpace(region);

        var datetimeNow = RoundUp(DateTimeOffset.Now, TimeSpan.FromMinutes(5));

        var result = await GetForecastDataAsync(region, datetimeNow, nextXHoursForAnExecutionWindow, estimatedExecutionDuration);

        return result.IsOptimalWindowNow;

    }

    private static DateTimeOffset RoundUp(DateTimeOffset dt, TimeSpan d)
    {
        return new DateTimeOffset((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Offset);
    }
}