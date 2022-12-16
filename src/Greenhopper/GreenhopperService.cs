using CarbonAware.Exceptions;
using CarbonAware.Model;
using Greenhopper.Core.Exceptions;
using Greenhopper.Core.Services;
using Greenhopper.Models;
using Greenhopper.SettingsConfiguration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Greenhopper;

/// <summary>
/// Service that finds the optimal window for the execution time suggested.
/// </summary>
public class GreenhopperService : IGreenhopperService
{
    private readonly ILogger<GreenhopperService> _logger;

    private readonly IForecastDataCollector _forecastDataCollector;

    private static readonly ActivitySource Activity = new(nameof(GreenhopperService));

    private readonly GrasshoperSettings _settings;

    /// <summary>
    /// Creates an instance of a <see cref="GreenhopperService"/>.
    /// </summary>
    /// <param name="loggerFactory">An instance used to configure the logging system and create <see cref="ILogger"/> instances.</param>
    /// <param name="forecastDataCollector">An instance of the service that can retrieve forecast data from Carbon Aware SDK.</param>
    /// <param name="configuration">The key/value application configuration.</param>
    public GreenhopperService(ILoggerFactory loggerFactory,
        IForecastDataCollector forecastDataCollector,
        IConfiguration configuration)
    {
        ExceptionExtensions.ThrowIfNull(loggerFactory);
        ExceptionExtensions.ThrowIfNull(forecastDataCollector);
        ExceptionExtensions.ThrowIfNull(configuration);

        _logger = loggerFactory.CreateLogger<GreenhopperService>();
        _forecastDataCollector = forecastDataCollector;
        _settings = configuration
                    .GetSection(GrasshoperSettings.Key)
                    .Get<GrasshoperSettings>();
    }

    /// <inheritdoc/>
    public async Task<OptimalWindowResponse> GetForecastDataAsync(string region, DateTimeOffset datetime, int executionTimeFrameInHours, int estimatedExecutionDurationInMinutes)
    {
        using var activity = Activity.StartActivity();

        ExceptionExtensions.ThrowIfNullOrWhiteSpace(region);
        ExceptionExtensions.ThrowIfOutsideBounds(executionTimeFrameInHours, 1, int.MaxValue);
        ExceptionExtensions.ThrowIfOutsideBounds(estimatedExecutionDurationInMinutes, 1, executionTimeFrameInHours * 60);
        ExceptionExtensions.ThrowIfOutsideBounds(datetime, DateTimeOffset.Now, DateTimeOffset.MaxValue);

        datetime = RoundUp(datetime, TimeSpan.FromMinutes(5));

        _logger.LogInformation("Requesting forecast for region '{region}' and datetime '{datetime}'", region, datetime);
        var forecastData = await _forecastDataCollector.GetAsync(region, datetime, executionTimeFrameInHours, estimatedExecutionDurationInMinutes);

        //no data
        if (forecastData == default)
        {
            _logger.LogError("No forecast data returned for region '{region}' and datetime '{datetimeNow}'.", region, datetime);
            return !_settings.OnNoForecastContinue
                ? throw new CarbonAwareException($"No forecast returned for region '{region}' and datetime '{datetime}' and '{nameof(GrasshoperSettings.OnNoForecastContinue)}' is false.")
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

        return await IsOptimalWindowNowAsync(_settings.ExecutionTimeFrameInHours, _settings.EstimatedExecutionDurationInMinutes);
    }

    /// <inheritdoc/>
    public async Task<bool> IsOptimalWindowNowAsync(int executionTimeFrameInHours, int estimatedExecutionDurationInMinutes)
    {
        using var activity = Activity.StartActivity();

        ExceptionExtensions.ThrowIfOutsideBounds(executionTimeFrameInHours, 1, int.MaxValue);
        ExceptionExtensions.ThrowIfOutsideBounds(estimatedExecutionDurationInMinutes, 1, executionTimeFrameInHours * 60);

        var region = Environment.GetEnvironmentVariable(GrasshoperSettings.REGION_NAME)?.Replace(" ", string.Empty).ToLower();
        ExceptionExtensions.ThrowIfNullOrWhiteSpace(region);

        return await IsOptimalWindowNowAsync(region, executionTimeFrameInHours, estimatedExecutionDurationInMinutes);

    }

    /// <inheritdoc/>
    public async Task<bool> IsOptimalWindowNowAsync(string region, int executionTimeFrameInHours, int estimatedExecutionDurationInMinutes)
    {
        using var activity = Activity.StartActivity();

        ExceptionExtensions.ThrowIfNullOrWhiteSpace(region);
        ExceptionExtensions.ThrowIfOutsideBounds(executionTimeFrameInHours, 1, int.MaxValue);
        ExceptionExtensions.ThrowIfOutsideBounds(estimatedExecutionDurationInMinutes, 1, executionTimeFrameInHours * 60);

        region = region.Replace(" ", string.Empty).ToLower();
        ExceptionExtensions.ThrowIfNullOrWhiteSpace(region);

        var datetimeNow = RoundUp(DateTimeOffset.Now, TimeSpan.FromMinutes(5));

        var result = await GetForecastDataAsync(region, datetimeNow, executionTimeFrameInHours, estimatedExecutionDurationInMinutes);

        return result.IsOptimalWindowNow;
    }

    private static DateTimeOffset RoundUp(DateTimeOffset dt, TimeSpan d)
    {
        return new DateTimeOffset((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Offset);
    }
}