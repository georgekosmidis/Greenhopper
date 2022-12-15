using CarbonAware.Model;

namespace Grasshopper.Models;

/// <summary>
/// A response object that contains the parsed result of 
/// the <see cref="EmissionsForecast"/> object returned
/// from the Carbon Aware SDK.
/// </summary>
public class OptimalWindowResponse
{
    /// <summary>
    /// True if the optimal window for the suggested payload is now.
    /// </summary>
    public bool IsOptimalWindowNow { get; internal set; }

    /// <summary>
    /// A <see cref="DateTimeOffset"/> indicating when the optimal window is.
    /// </summary>
    public DateTimeOffset OptimalWindow { get; internal set; }

    /// <summary>
    /// The <see cref="EmissionsForecast"/> data returned from Carbon Aware SDK.
    /// </summary>
    public EmissionsForecast Data { get; internal set; } = new EmissionsForecast();
}
