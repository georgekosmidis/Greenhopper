using CarbonAware.AzureFunction.Services.HostingHostBuilderExtensions;
using Microsoft.Extensions.Hosting;

new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureCarbonAwareApp()
    .Build()
    .Run();
