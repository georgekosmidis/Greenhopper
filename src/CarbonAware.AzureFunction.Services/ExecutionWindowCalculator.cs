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

    private readonly CarbonAwareAzureFunctionConfiguration _configVars;

    public ExecutionWindowCalculator(ILoggerFactory loggerFactory,
        IForecastAggregator forecastAggregator,
        IConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger<ExecutionWindowCalculator>();
        _forecastAggregator = forecastAggregator ?? throw new ArgumentNullException(nameof(forecastAggregator));
        _configVars = configuration
                    .GetSection(CarbonAwareAzureFunctionConfiguration.Key)
                    .Get<CarbonAwareAzureFunctionConfiguration>();
    }

    /// <summary>
    /// Calculates if the current time is optimal for executing your Azure Function workload.
    /// The engine searches for the optimal window of executing 
    /// a <see cref="CarbonAwareAzureFunctionConfiguration.EstimatedExecutionDuration"/> minutes workload
    /// between now and (now+hours) defined in <see cref="CarbonAwareAzureFunctionConfiguration.NextXHoursForAnExecutionWindow"/>.
    /// The values are available as config values in appsettings or in Azure Function Configuration.
    /// </summary>
    /// <returns>True if the optimal window is now, false for every other reason.</returns>
    public async Task<bool> IsNowOptimal()
    {

        return await IsNowOptimalAsync(_configVars.EstimatedExecutionDuration, _configVars.NextXHoursForAnExecutionWindow);
    }

    /// <summary>
    /// Calculates if the current time is optimal for executing your Azure Function workload.
    /// The engine searches for the optimal window of executing 
    /// a <paramref name="estimatedExecutionDuration"/> minutes workload
    /// between now and (now+hours) defined in <paramref name="nextXHoursForAnExecutionWindow"/>.
    /// The values are available as config values in appsettings or in Azure Function Configuration.
    /// </summary>
    /// <param name="estimatedExecutionDuration">A parameter indicating the estimated minutes of execution of the Azure Function.</param>
    /// <param name="nextXHoursForAnExecutionWindow">A parameter indicating the timespan within the execution window should be searched.</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException">Xhen <see cref="CarbonAwareAzureFunctionConfiguration.REGION_NAME"/> is not defined.</exception>
    /// <exception cref="CarbonAwareException">When no forecast data are returned and <see cref="CarbonAwareAzureFunctionConfiguration.OnNoForecastExecute"/> is set to false.</exception>
    public async Task<bool> IsNowOptimalAsync(int estimatedExecutionDuration, int nextXHoursForAnExecutionWindow)
    {
        using var activity = Activity.StartActivity();

        var region = Environment.GetEnvironmentVariable(CarbonAwareAzureFunctionConfiguration.REGION_NAME)?.Replace(" ", string.Empty).ToLower()
            ?? throw new NullReferenceException(nameof(CarbonAwareAzureFunctionConfiguration.REGION_NAME));

        var datetimeNow = RoundUp(DateTimeOffset.Now, TimeSpan.FromMinutes(5));

        _logger.LogInformation("Requesting forecast for region '{region}' and datetime '{datetimeNow}'", region, datetimeNow);

        var foreCastData = await _forecastAggregator.GetCurrentForecastDataAsync(new EmissionsForecastCurrentDTO
        {
            MultipleLocations = new string[] { region },
            Start = datetimeNow,
            End = datetimeNow.AddHours(nextXHoursForAnExecutionWindow),
            Duration = estimatedExecutionDuration
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
            return !_configVars.OnNoForecastExecute
                ? throw new CarbonAwareException($"No forecast returned for region '{region}' and datetime '{datetimeNow}' and '{nameof(CarbonAwareAzureFunctionConfiguration.OnNoForecastExecute)}' is false.")
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

    private static DateTimeOffset RoundUp(DateTimeOffset dt, TimeSpan d)
    {
        return new DateTimeOffset((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Offset);
    }
}
