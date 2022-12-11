# An Azure Function with a timer trigger that executes only when the emissions are below a threshold
---

_*TODO*: Documentation is missing_

## Points of interest
Up until a detailed documentation is ready, the following bullets are worth your attention.

1. Notice the `.ConfigureCarbonAwareApp()` in [Program.cs](https://github.com/georgekosmidis/carbon-aware-azure-function/blob/dev/samples/carbon-aware-azure-function/Program.cs), that is configuring everything
1. Notice the [appsettings.json](https://github.com/georgekosmidis/carbon-aware-azure-function/blob/dev/samples/carbon-aware-azure-function/appsettings.json), you need everything in there
1. Check the use of `_locationEmissions.ContinueExecutionAsync()` in [Function1.cs](https://github.com/georgekosmidis/carbon-aware-azure-function/blob/dev/samples/carbon-aware-azure-function/Function1.cs) 

##  References
1. [Carbon Aware Azure Function Service](https://github.com/georgekosmidis/carbon-aware-azure-function/tree/dev/src/CarbonAware.AzureFunction.Services)
2. [How to create an account in WattTime](https://www.watttime.org/api-documentation/#register-new-user)
