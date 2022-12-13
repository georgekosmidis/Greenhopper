namespace CarbonAware.AzureFunction.Services;

public interface IExecutionWindowCalculatorService
{
    Task<bool> IsNowOptimal();
}