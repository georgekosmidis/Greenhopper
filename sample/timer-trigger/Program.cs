using Grasshopper.HostingHostBuilderExtensions;
using Microsoft.Extensions.Hosting;

new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureGrasshopper()
    .Build()
    .Run();
