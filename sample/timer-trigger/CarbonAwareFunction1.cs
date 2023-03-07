using Greenhopper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CarbonAware.AzureFunction;

public class CarbonAwareFunction1
{
    private readonly ILogger<CarbonAwareFunction1> _logger;

    private static readonly ActivitySource Activity = new(nameof(CarbonAwareFunction1));

    private readonly IGreenhopperService _greenhopper;

    public CarbonAwareFunction1(ILoggerFactory loggerFactory, IGreenhopperService greenhopper)
    {
        _logger = loggerFactory.CreateLogger<CarbonAwareFunction1>();
        _greenhopper = greenhopper;
    }

    [Function("CarbonAwareFunction1")]
    public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo timerInfo)
    {
        using var activity = Activity.StartActivity(nameof(CarbonAwareFunction1.Run));
        if (!await _greenhopper.IsOptimalWindowNowAsync())
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
