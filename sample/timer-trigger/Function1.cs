using Grasshopper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CarbonAware.AzureFunction;

public class Function1
{
    private readonly ILogger<Function1> _logger;

    private static readonly ActivitySource Activity = new(nameof(Function1));

    private readonly IOptimalWindowCalculatorService _locationEmissions;

    public Function1(ILoggerFactory loggerFactory, IOptimalWindowCalculatorService locationEmissions)
    {
        _logger = loggerFactory.CreateLogger<Function1>();
        _locationEmissions = locationEmissions;
    }

    [Function("CarbonAwareFunction")]
    public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo timerInfo)
    {
        using var activity = Activity.StartActivity();
        if (!await _locationEmissions.IsOptimalWindowNowAsync())
        {
            _logger.LogWarning("No execution for now!");
            return;
        }
        _logger.LogInformation("Executing!!!");

        //Write your payload here
    }
}

public class TimerInfo
{
    public TimerScheduleStatus? ScheduleStatus { get; set; }

    public bool IsPastDue { get; set; }
}

public class TimerScheduleStatus
{
    public DateTime Last { get; set; }

    public DateTime Next { get; set; }

    public DateTime LastUpdated { get; set; }
}
