namespace CarbonAware.AzureFunction.Services;

public interface IRegionEmissionsService
{
    Task<bool> ContinueExecutionAsync();
}