using Greenhopper.Models;
using Greenhopper.SettingsConfiguration;

namespace Greenhopper;

/// <summary>
/// An abstraction for the Optimal Window Calculator
/// </summary>
public interface IGreenhopperService
{
    /// <summary>
    /// Checks for an optimal window based on the parameters passed, and returns the result.
    /// </summary>
    /// <param name="region">An Azure Region.</param>
    /// <param name="datetime">A future datetime offset that the forecast.</param>
    /// <param name="executionTimeFrameInHours">The hours after <paramref name="datetime"/> that will be searched for an available optimal window.</param>
    /// <param name="estimatedExecutionDurationInMinutes">The estimated execution time in minutes; the algorithm searches for an optimal execution window of <paramref name="estimatedExecutionDurationInMinutes"/> minutes.</param>
    /// <returns>An <see cref="OptimalWindowResponse"/> object.</returns>
    Task<OptimalWindowResponse> GetForecastDataAsync(string region, DateTimeOffset datetime, int executionTimeFrameInHours, int estimatedExecutionDurationInMinutes);

    /// <summary>
    /// Calculates if the current run is within the optimal window of executing a payload.
    /// The engine searches for the optimal window of executing 
    /// a <see cref="GrasshoperSettings.EstimatedExecutionDurationInMinutes"/> minutes payload
    /// between <see cref="DateTimeOffset.Now"/> and <see cref="DateTimeOffset.Now"/> plus <see cref="GrasshoperSettings.ExecutionTimeFrameInHours"/> hours.
    /// The values are available as config values in the <see cref="GrasshoperSettings"/> section of the appsettings.json,
    /// and can be overridden in Azure Function Configuration.
    /// </summary>
    /// <returns><code>true</code> if the optimal window is now, <code>false</code> for every other reason.</returns>
    Task<bool> IsOptimalWindowNowAsync();

    /// <summary>
    /// Calculates if the current run is within the optimal window of executing a payload.
    /// The engine searches for the optimal window of executing 
    /// a <paramref name="estimatedExecutionDurationInMinutes"/> minutes payload
    /// between <see cref="DateTimeOffset.Now"/> and <see cref="DateTimeOffset.Now"/> plus <paramref name="executionTimeFrameInHours"/> hours.
    /// </summary>
    /// <param name="estimatedExecutionDurationInMinutes">A parameter indicating the estimated minutes of execution of the Azure Function.</param>
    /// <param name="executionTimeFrameInHours">A parameter indicating the timespan within the execution window should be searched.</param>
    /// <returns><code>true</code> if the optimal window is now, <code>false</code> for every other reason.</returns>
    Task<bool> IsOptimalWindowNowAsync(int executionTimeFrameInHours, int estimatedExecutionDurationInMinutes);

    /// <summary>
    /// Calculates if the current run is within the optimal window of executing a payload.
    /// The engine searches for the optimal window of executing 
    /// a <paramref name="estimatedExecutionDurationInMinutes"/> minutes payload
    /// between <see cref="DateTimeOffset.Now"/> and <see cref="DateTimeOffset.Now"/> plus <paramref name="executionTimeFrameInHours"/> hours.
    /// </summary>
    /// <param name="region">An Azure Region.</param>
    /// <param name="estimatedExecutionDurationInMinutes">A parameter indicating the estimated minutes of execution of the Azure Function.</param>
    /// <param name="executionTimeFrameInHours">A parameter indicating the timespan within the execution window should be searched.</param>
    /// <returns><code>true</code> if the optimal window is now, <code>false</code> for every other reason.</returns>

    Task<bool> IsOptimalWindowNowAsync(string region, int executionTimeFrameInHours, int estimatedExecutionDurationInMinutes);
}