namespace CarbonAware.AzureFunction.Services.SettingsConfiguration;
public class CarbonAwareAzureFunctionConfiguration
{
    public const string REGION_NAME = "REGION_NAME";

    public const string Key = "carbonAwareFunctionVars";

    /// <summary>
    /// Gets or sets the estimated execution duration of the Azure Function in minutes
    /// </summary>
    public int EstimatedExecutionDuration { get; set; }

    /// <summary>
    /// Searches the optimal time to run the Azure Function in the next X hours
    /// </summary>
    public int HoursForExecutionWindowSearch { get; set; }

    /// <summary>
    /// Gets or sets a flag signaling the function to run when no emissions returned.
    /// </summary>
    public bool OnNoForecastExecute { get; set; }
}
