using Grasshopper.Models;
using Grasshopper.SettingsConfiguration;

namespace Grasshopper;

/// <summary>
/// An abstraction for the Optimal Window Calculator
/// </summary>
public interface IOptimalWindowCalculatorService
{
    /// <summary>
    /// Checks for an optimal window based on the parameters passed, and returns the result.
    /// </summary>
    /// <param name="region">An Azure Region.</param>
    /// <param name="datetime">A future datetime offset that the forecast.</param>
    /// <param name="nextXHoursForAnExecutionWindow">The hours after <paramref name="datetime"/> that will be searched for an available optimal window.</param>
    /// <param name="estimatedExecutionDuration">The estimated execution time in minutes; the algorithm searches for an optimal execution window of <paramref name="estimatedExecutionDuration"/> minutes.</param>
    /// <returns>An <see cref="OptimalWindowResponse"/> object.</returns>
    Task<OptimalWindowResponse> GetForecastDataAsync(string region, DateTimeOffset datetime, int nextXHoursForAnExecutionWindow, int estimatedExecutionDuration);

    /// <summary>
    /// Calculates if the current run is within the optimal window of executing a payload.
    /// The engine searches for the optimal window of executing 
    /// a <see cref="GrasshoperSettings.EstimatedExecutionDuration"/> minutes payload
    /// between <see cref="DateTimeOffset.Now"/> and <see cref="DateTimeOffset.Now"/> plus <see cref="GrasshoperSettings.NextXHoursForAnExecutionWindow"/> hours.
    /// The values are available as config values in the <see cref="GrasshoperSettings"/> section of the appsettings.json,
    /// and can be overridden in Azure Function Configuration.
    /// </summary>
    /// <returns><code>true</code> if the optimal window is now, <code>false</code> for every other reason.</returns>
    Task<bool> IsOptimalWindowNowAsync();

    /// <summary>
    /// Calculates if the current run is within the optimal window of executing a payload.
    /// The engine searches for the optimal window of executing 
    /// a <paramref name="estimatedExecutionDuration"/> minutes payload
    /// between <see cref="DateTimeOffset.Now"/> and <see cref="DateTimeOffset.Now"/> plus <paramref name="nextXHoursForAnExecutionWindow"/> hours.
    /// </summary>
    /// <param name="estimatedExecutionDuration">A parameter indicating the estimated minutes of execution of the Azure Function.</param>
    /// <param name="nextXHoursForAnExecutionWindow">A parameter indicating the timespan within the execution window should be searched.</param>
    /// <returns><code>true</code> if the optimal window is now, <code>false</code> for every other reason.</returns>
    Task<bool> IsOptimalWindowNowAsync(int nextXHoursForAnExecutionWindow, int estimatedExecutionDuration);
}