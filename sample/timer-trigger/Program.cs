using Greenhopper.HostingHostBuilderExtensions;
using Microsoft.Extensions.Hosting;

new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureGreenhopper()
    .Build()
    .Run();
