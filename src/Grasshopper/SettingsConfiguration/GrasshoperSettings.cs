namespace Grasshopper.SettingsConfiguration;

/// <summary>
/// The settings required by Grasshopper to run.
/// </summary>
public class GrasshoperSettings
{
    /// <summary>
    /// The environment variable name of the Azure Region.
    /// </summary>
    public const string REGION_NAME = "REGION_NAME";

    /// <summary>
    /// The Settings key for the Carbon Aware SDK variables.
    /// </summary>
    public const string Key = "carbonAwareFunctionVars";

    /// <summary>
    /// Gets or sets the estimated execution duration of the Azure Function in minutes
    /// </summary>
    public int EstimatedExecutionDuration { get; set; }

    /// <summary>
    /// Searches the optimal time to run the Azure Function in the next X hours
    /// </summary>
    public int NextXHoursForAnExecutionWindow { get; set; }

    /// <summary>
    /// Gets or sets a flag signaling the function to run when no emissions returned.
    /// </summary>
    public bool OnNoForecastExecute { get; set; }
}
