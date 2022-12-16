using CarbonAware.Model;

namespace Greenhopper.Core.Services;

/// <summary>
/// An abstraction for an object that retrieves emissions forecast data.
/// </summary>
public interface IForecastDataCollector
{
    /// <summary>
    /// Retrieves forecast data using the Carbon Aware SDK.
    /// </summary>
    /// <param name="region">An Azure Region.</param>
    /// <param name="datetime">A future datetime offset that the forecast.</param>
    /// <param name="nextXHoursForAnExecutionWindow">The hours after <paramref name="datetime"/> that will be searched for an available optimal window.</param>
    /// <param name="estimatedExecutionDuration">The estimated execution time in minutes; the algorithm searches for an optimal execution window of <paramref name="estimatedExecutionDuration"/> minutes.</param>
    /// <returns>An <see cref="EmissionsForecast"/> object.</returns>
    Task<EmissionsForecast> GetAsync(string region, DateTimeOffset datetime, int nextXHoursForAnExecutionWindow, int estimatedExecutionDuration);
}