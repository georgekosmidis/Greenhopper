using CarbonAware.Aggregators.CarbonAware;
using CarbonAware.Aggregators.Forecast;

namespace Greenhopper.Models;

/// <summary>
/// A Data Tranfer Object that holds the parameters  
/// <see cref="IForecastAggregator.GetCurrentForecastDataAsync(CarbonAwareParameters)"/> needs.
/// </summary>
public class EmissionsForecastCurrentDTO : CarbonAwareParametersBaseDTO
{
    /// <summary>String array of named locations</summary>
    /// <example>westus</example>
    public override string[]? MultipleLocations { get; set; }

    /// <summary>
    /// Start time boundary of forecasted data points.Ignores current forecast data points before this time.
    /// Defaults to the earliest time in the forecast data.
    /// </summary>
    /// <example>2022-12-15T09:00:00Z</example>
    public override DateTimeOffset? Start { get; set; }

    /// <summary>
    /// End time boundary of forecasted data points. Ignores current forecast data points after this time.
    /// Defaults to the latest time in the forecast data.
    /// </summary>
    /// <example>2022-12-15T21:00:00Z</example>
    public override DateTimeOffset? End { get; set; }

    /// <summary>
    /// The estimated duration (in minutes) of the workload.
    /// Defaults to the duration of a single forecast data point.
    /// </summary>
    /// <example>15</example>
    public override int? Duration { get; set; }
}