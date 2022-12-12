namespace CarbonAware.AzureFunction.Services;

public interface IExecutionWindowCalculator
{
    Task<bool> IsNowOptimal();
}