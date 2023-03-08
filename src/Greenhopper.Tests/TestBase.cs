using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GSF.CarbonAware.Handlers;
using GSF.CarbonAware.Models;
using Greenhopper.Core.Cache;
using Microsoft.Extensions.Logging.Abstractions;

namespace Greenhopper.Core.Services.Tests;

public abstract class TestBase
{

    protected static Mock<ICacheManager> CreateLoggerFactory<T>(T data)
    {
        var cache = new Mock<ICacheManager>();

        cache.Setup(x => x.AddOrGetExisting<T>(It.IsAny<string>(), It.IsAny<Func<Task<T>>>(), It.IsAny<int>()))
             .ReturnsAsync<ICacheManager, T>(data);

        return cache;
    }

    protected static Mock<ICacheManager> CreateCacheMock<T>(T data)
    {
        var cache = new Mock<ICacheManager>();

        cache.Setup(x => x.AddOrGetExisting<T>(It.IsAny<string>(), It.IsAny<Func<Task<T>>>(), It.IsAny<int>()))
             .ReturnsAsync<ICacheManager, T>(data);

        return cache;
    }

    protected static Mock<IForecastHandler> CreateForecastHandlerMock(IEnumerable<EmissionsForecast> data)
    {
        var handler = new Mock<IForecastHandler>();
        handler.Setup(x => x.GetCurrentForecastAsync(It.IsAny<string[]>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>()))
               .Callback((string[] locations, DateTimeOffset? dataStartAt, DateTimeOffset? dataEndAt, int? windowSize) =>
               {
                   Assert.IsNotNull(locations);
                   Assert.IsTrue(locations.Length > 0);
               })
               .ReturnsAsync(data);

        return handler;
    }

    protected ForecastDataCollector CreateForecastDataCollector(IEnumerable<EmissionsForecast> data)
    {
        var cache = CreateCacheMock(data);
        var handler = CreateForecastHandlerMock(data);

        return new ForecastDataCollector(
            new NullLoggerFactory(),
            handler.Object,
            cache.Object
        );
    }
}