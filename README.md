[![Testing](https://github.com/georgekosmidis/greenhopper/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/georgekosmidis/greenhopper/actions/workflows/build-and-test.yml) [![Update carbon-aware SDK](https://github.com/georgekosmidis/greenhopper/actions/workflows/update-carbon-aware-sdk.yml/badge.svg)](https://github.com/georgekosmidis/greenhopper/actions/workflows/update-carbon-aware-sdk.yml)

[![Nuget Publishment](https://github.com/georgekosmidis/greenhopper/actions/workflows/nuget-publishment.yml/badge.svg)](https://github.com/georgekosmidis/greenhopper/actions/workflows/nuget-publishment.yml) [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Greenhopper.svg?logo=nuget)](https://www.nuget.org/packages/Greenhopper) 

![](https://raw.githubusercontent.com/georgekosmidis/Greenhopper/main/docs/greenhopper_simple.png)

# Greenhopper - Decarbonize your non-critical Azure workloads!

Greenhopper is a small >NET library that adds carbon aware capabilities to your Azure Function, converting your non-critical workload to a sustainable one that runs only when the region electricity is greener.

## How it works

It uses emissions forecast from [Carbon Aware SDK](https://github.com/Green-Software-Foundation/carbon-aware-sdk) that predicts an optimal window of execution for an estimated workload of X minutes within a specified time frame and for a specific Azure Region. It then uses that prediction to decide if the current execution is within the optimal window and subsequently allows or not the workload execution to complete.

> The question it answer is, "***I want to run an Azure Function in region West US for 5 minutes sometime within the next 8 hours. Is now the time?***"

## Getting started

> If you prefere a to get you started, he it is [CarbonAware.AzureFunction.Sample](https://github.com/georgekosmidis/carbon-aware-azure-function/tree/main/sample/timer-trigger)

Getting started requires four steps:

1. Install the [Greenhopper](https://www.nuget.org/packages/Greenhopper/) nuget package 
1. Add or update your `appsetings.json`
1. Configure Greenhopper in `Program.cs` 
1. Use DI to ask the question!

### Add the nuget package to your project 

Install the [Greenhopper](https://www.nuget.org/packages/Greenhopper/) nuget package, using any of your favorite ways.

### Add or update your `appsetings.json`

```json
{
  "DataSources:ForecastDataSource": "WattTime",
  "DataSources:Configurations:WattTime:Type": "WattTime",
  "DataSources:Configurations:WattTime:Username": "greenhopper",
  "DataSources:Configurations:WattTime:Password": "$gr33nh0pp3r",
  "DataSources:Configurations:WattTime:BaseURL": "https://api2.watttime.org/v2/",
  "DataSources:Configurations:WattTime:Proxy:UseProxy": false,
  "greenhopper:EstimatedExecutionDurationInMinutes": 5, // my func will run for aprox X mins (searches for the optimal time to run an X mins payload)
  "greenhopper:ExecutionTimeFrameInHours": 8, // my func needs to run in the next X hours (searches the optimal time to run an X mins payload in the next X hours)
  "greenhopper:OnNoForecastContinue": false
}
```

> For more configuration options, like using [ElectricityMaps](https://www.electricitymaps.com/) instead of [WattTime](https://www.watttime.org/) or a test dataset, follow the Carbon Aware SDK appsettings template: https://github.com/Green-Software-Foundation/carbon-aware-sdk/blob/dev/src/CarbonAware.WebApi/src/appsettings.Development.json.template

### Configure Greenhopper in `Program.cs`

```csharp
using Greenhopper.HostingHostBuilderExtensions; // <-- Add the reference
using Microsoft.Extensions.Hosting;

new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureGreenhopper()                     // <-- Add the configuration
    .Build()
    .Run();
```

### Use DI to ask the question!
```csharp
public class CarbonAwareFunction1
{
    private readonly IGreenhopperService _greenhopper;

    public Function1(IGreenhopperService greenhopper)
    {
        _greenhopper = greenhopper;
    }

    [Function("CarbonAwareFunction1")]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo)
    {
        //Reads the environment variable REGION_NAME 
        // from either the local environment variables or from Azure predefined ones
        // when deployed to Azure,
        // and the ExecutionTimeFrameInHours and EstimatedExecutionDurationInMinutes 
        // from either appsettings.json or Azure Function Configuration
        if (!await _greenhopper.IsOptimalWindowNowAsync())
        {
            return;
        }
        
        // - OR -
        
        //Reads the environment variable REGION_NAME internally
        // from either the local environment variables or from Azure predefined ones
        // when deployed to Azure
        if (!await _greenhopper.IsOptimalWindowNowAsync(ExecutionTimeFrameInHours:8, EstimatedExecutionDurationInMinutes:5))
        {
            return;
        }
        //Write your code here
    }
}
```

## Testing vs Production
[Carbon Aware SDK](https://github.com/Green-Software-Foundation/carbon-aware-sdk) (and subsequently Greenhopper) is depended on 3rd party data providers such as [WattTime](https://www.watttime.org/) or [ElectricityMaps](https://www.electricitymaps.com/) to get the emissions for the region your Azure Function is deployed. 
For testing and quick onboarding, Greenhopper uses [WattTime](https://www.watttime.org/) that gives you unlimited access to one Azure Region ("West US") to try out the library and deploy your workloads.

> The username/password included here is for testing only; for production you need to create your own account using the following `POST` request (there is no UI!):

```
POST /v2/register HTTP/1.1
Host: api2.watttime.org
Content-Type: application/json
Content-Length: 143

{
    "username": "YOUR USERNAME",
    "password": "YOUR PASSWORD ", //at least 8 characters, with at least 1 of each alpha, number and special characters
    "email": "YOUR EMAIL",
    "org": "YOUR NAME OR ORG"
}
```

> You can also import [this Postman Collection](https://raw.githubusercontent.com/georgekosmidis/Greenhopper/main/docs/WattTime%20Account.postman_collection.json).

## Ideation
The idea of using [Carbon Aware SDK](https://github.com/Green-Software-Foundation/carbon-aware-sdk) was born in [Azure Developer Community Conference 2022](https://azuredev.org/) from [Ralf Richter](https://github.com/entwickler42). The first introduction was in that conference during the [Sustainability: Carbon Aware Azure Functions
Live Stream](https://azuredev.org/sessions/sustainability-carbon-aware-azure-functions) session with a different name, and shortly after the implementation completed.

##  References
1. [Carbon Aware Azure Function Sample (Timer Trigger)](https://github.com/georgekosmidis/carbon-aware-azure-function/tree/main/sample/timer-trigger)
2. [How to create an account in WattTime](https://www.watttime.org/api-documentation/#register-new-user)
3. [Carbon Aware SDK repo](https://github.com/Green-Software-Foundation/carbon-aware-sdk)
4. [Azure Developer Community Conference 2022](https://azuredev.org/)
