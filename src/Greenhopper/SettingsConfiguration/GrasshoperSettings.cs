namespace Greenhopper.SettingsConfiguration;

/// <summary>
/// The settings required by Greenhopper to run.
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
    public const string Key = "greenhopper";

    /// <summary>
    /// Gets or sets the estimated execution duration of the Azure Function in minutes
    /// </summary>
    public int EstimatedExecutionDurationInMinutes { get; set; }

    /// <summary>
    /// Searches the optimal time to run the Azure Function in the next X hours
    /// </summary>
    public int ExecutionTimeFrameInHours { get; set; }

    /// <summary>
    /// Gets or sets a flag signaling the function to run when no emissions returned.
    /// </summary>
    public bool OnNoForecastContinue { get; set; }
}
