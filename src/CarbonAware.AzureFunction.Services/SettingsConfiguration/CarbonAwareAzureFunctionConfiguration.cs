namespace CarbonAware.AzureFunction.Services.SettingsConfiguration;
public class CarbonAwareAzureFunctionConfiguration
{
    public const string REGION_NAME = "REGION_NAME";

    public const string Key = "carbonAwareFunctionVars";

    /// <summary>
    /// Gets or sets the emissions threshold above which the function should abandon.
    /// </summary>
    public double EmmissionsThreshold { get; set; }

    /// <summary>
    /// Gets or sets a flag signaling the function to run when no emissions returned.
    /// </summary>
    public bool OnNoEmissionsContinue { get; set; }
}
