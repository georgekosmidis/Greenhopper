using CarbonAware.Aggregators.Emissions;
using CarbonAware.AzureFunction.Services.SettingsConfiguration;
using CarbonAware.AzureFunction.Services.Models;
using CarbonAware.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace CarbonAware.AzureFunction.Services;

/// <summary>
/// Service that tracks emissions for the current Azure Function Region 
/// </summary>
public class RegionEmissionsService : IRegionEmissionsService
{
    private readonly ILogger<RegionEmissionsService> _logger;

    private readonly IEmissionsAggregator _emissionsAggregator;

    private static readonly ActivitySource Activity = new ActivitySource(nameof(RegionEmissionsService));

    private readonly IConfiguration _configuration;

    public RegionEmissionsService(ILoggerFactory loggerFactory, IEmissionsAggregator emissionsAggregator, IConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger<RegionEmissionsService>();
        _emissionsAggregator = emissionsAggregator ?? throw new ArgumentNullException(nameof(emissionsAggregator));
        _configuration = configuration;
    }

    /// <summary>
    /// Rt
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="CarbonAwareException"></exception>
    public async Task<bool> ContinueExecutionAsync()
    {
        using (var activity = Activity.StartActivity())
        {
            var configVars = _configuration.GetSection(CarbonAwareAzureFunctionConfiguration.Key).Get<CarbonAwareAzureFunctionConfiguration>();

            var region = Environment.GetEnvironmentVariable(CarbonAwareAzureFunctionConfiguration.REGION_NAME)?.Replace(" ", string.Empty).ToLower()
                ?? throw new NullReferenceException(nameof(CarbonAwareAzureFunctionConfiguration.REGION_NAME));

            var offset = DateTimeOffset.Now;

            _logger.LogInformation($"Requesting emissions for region '{region}' and datetime '{offset}'");

            var parameters = new EmissionsDataForLocationsParametersDTO
            {
                MultipleLocations = new string[] { region },
                Start = offset,
                End = offset.AddMilliseconds(1)
            };

            var response = await _emissionsAggregator.GetEmissionsDataAsync(parameters);
            if (!response.Any())
            {
                _logger.LogError($"No emmisions returned for region '{region}' and datetime '{offset}'.");
                if (!configVars.OnNoEmissionsContinue)
                {
                    throw new CarbonAwareException($"No emmisions returned for region '{region}' and datetime '{offset}' and '{nameof(configVars.OnNoEmissionsContinue)}' is false.");
                }
                _logger.LogWarning($"Run continues because of {nameof(configVars.OnNoEmissionsContinue)} application setting.");
            }

            //don't serialize for nothing
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var options = new JsonSerializerOptions();
                options.WriteIndented = true;
                var stringResponse = JsonSerializer.Serialize(response, options);
                _logger.LogDebug($"Emmisions for '{region}' and {offset} offset UTC: {Environment.NewLine}{stringResponse}");
            }

            var lastEmissions = response.Last();
            if (configVars.EmmissionsThreshold < lastEmissions.Rating)
            {
                _logger.LogInformation($"Execution is halted! Emmisions for '{region}' and datetime '{lastEmissions.Time}' are {lastEmissions.Rating} " +
                    $"which is above the {nameof(configVars.EmmissionsThreshold)} application setting ({configVars.EmmissionsThreshold})");

                return false;
            }

            _logger.LogInformation($"Starting Execution! Emmisions for '{region}' and datetime '{lastEmissions.Time}' are {lastEmissions.Rating} " +
                $"which is below the {nameof(configVars.EmmissionsThreshold)} application setting ({configVars.EmmissionsThreshold})");

            return true;
        }
    }
}
