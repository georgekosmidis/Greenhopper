using CarbonAware.Aggregators.Forecast;
using CarbonAware.AzureFunction.Services.Models;
using CarbonAware.AzureFunction.Services.SettingsConfiguration;
using CarbonAware.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace CarbonAware.AzureFunction.Services;

/// <summary>
/// Service that tracks emissions for the current Azure Function Region 
/// </summary>
public class ExecutionWindowCalculator : IExecutionWindowCalculator
{
    private readonly ILogger<ExecutionWindowCalculator> _logger;

    private readonly IForecastAggregator _forecastAggregator;

    private static readonly ActivitySource Activity = new(nameof(ExecutionWindowCalculator));

    private readonly IConfiguration _configuration;

    public ExecutionWindowCalculator(ILoggerFactory loggerFactory,
        IForecastAggregator forecastAggregator,
        IConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger<ExecutionWindowCalculator>();
        _forecastAggregator = forecastAggregator ?? throw new ArgumentNullException(nameof(forecastAggregator));
        _configuration = configuration;
    }

    /// <summary>
    /// Calculates if the current time is optimal for executing your Azure Function Payload
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="CarbonAwareException"></exception>
    public async Task<bool> IsNowOptimal()
    {
        using var activity = Activity.StartActivity();
        var configVars = _configuration.GetSection(CarbonAwareAzureFunctionConfiguration.Key).Get<CarbonAwareAzureFunctionConfiguration>();

        var region = Environment.GetEnvironmentVariable(CarbonAwareAzureFunctionConfiguration.REGION_NAME)?.Replace(" ", string.Empty).ToLower()
            ?? throw new NullReferenceException(nameof(CarbonAwareAzureFunctionConfiguration.REGION_NAME));

        var datetimeNow = RoundUp(DateTimeOffset.Now, TimeSpan.FromMinutes(5));

        _logger.LogInformation("Requesting forecast for region '{region}' and datetime '{datetimeNow}'", region, datetimeNow);

        var foreCastData = await _forecastAggregator.GetCurrentForecastDataAsync(new EmissionsForecastCurrentDTO
        {
            MultipleLocations = new string[] { region },
            Start = datetimeNow,
            End = datetimeNow.AddHours(configVars.HoursForExecutionWindowSearch),
            Duration = configVars.EstimatedExecutionDuration
        });

        //log if debug is requested
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var stringforeCastData = JsonSerializer.Serialize(foreCastData, new JsonSerializerOptions
            {
                WriteIndented = true
            });
#pragma warning disable CA2254 // Template should be a static expression
            _logger.LogDebug($"Forecast data for '{region}' and {datetimeNow}: {Environment.NewLine}{stringforeCastData}");
#pragma warning restore CA2254 // Template should be a static expression 
        }

        //no data
        if (!foreCastData.Any())
        {
            _logger.LogError("No forecast data returned for region '{region}' and datetime '{datetimeNow}'.", region, datetimeNow);
            return !configVars.OnNoForecastExecute
                ? throw new CarbonAwareException($"No forecast returned for region '{region}' and datetime '{datetimeNow}' and '{nameof(configVars.OnNoForecastExecute)}' is false.")
                : false;
        }

        //no optimal window
        if (!foreCastData.First().OptimalDataPoints.Any())
        {
            _logger.LogWarning("No optimal execution window in sight! (try a bigger one?)");
            return false;
        }

        //sweet spot?
        if (foreCastData.First().OptimalDataPoints.Any(x => x.Time == datetimeNow))
        {
            //execute
            _logger.LogInformation("In optimal window, starting execution!");
            return true;
        }

        _logger.LogInformation("Execution skipped; Next probable window of execution is at {time}.", foreCastData.First().OptimalDataPoints.First().Time);
        return false;
    }

    private DateTimeOffset RoundUp(DateTimeOffset dt, TimeSpan d)
    {
        return new DateTimeOffset((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Offset);
    }
}
