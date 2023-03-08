using Microsoft.VisualStudio.TestTools.UnitTesting;
using GSF.CarbonAware.Models;
using Greenhopper.Core.Cache;

namespace Greenhopper.Core.Services.Tests;

[TestClass()]
public sealed class ForecastDataCollectorTests : TestBase
{
    [TestMethod]
    [ExpectedException(typeof(GreenhopperException))]
    public async Task GetAsync_EmptyRegion_Exception()
    {
        var data = new List<EmissionsForecast>();
        var dataCollector = CreateForecastDataCollector(data);
        await dataCollector.GetAsync(" ", DateTimeOffset.UtcNow, 1, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(GreenhopperException))]
    public async Task GetAsync_DateTimeEarlierThanNow_Exception()
    {
        var data = new List<EmissionsForecast>();
        var dataCollector = CreateForecastDataCollector(data);
        await dataCollector.GetAsync("westus", DateTimeOffset.UtcNow.AddMilliseconds(-1), 1, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(GreenhopperException))]
    public async Task GetAsync_HoursSmallerThanOne_Exception()
    {
        var data = new List<EmissionsForecast>();
        var dataCollector = CreateForecastDataCollector(data);
        await dataCollector.GetAsync("westus", DateTimeOffset.UtcNow, 0, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(GreenhopperException))]
    public async Task GetAsync_ExecutionSmallerThanOne_Exception()
    {
        var data = new List<EmissionsForecast>();
        var dataCollector = CreateForecastDataCollector(data);
        await dataCollector.GetAsync("westus", DateTimeOffset.UtcNow, 1, 0);
    }

    [TestMethod]
    [ExpectedException(typeof(GreenhopperException))]
    public async Task GetAsync_NoResults_Exception()
    {
        var data = new List<EmissionsForecast>();
        var dataCollector = CreateForecastDataCollector(data);
        await dataCollector.GetAsync("westus", DateTimeOffset.UtcNow.AddDays(1), 1, 1);
    }

    [TestMethod]
    public async Task GetAsync()
    {
        var data = new List<EmissionsForecast> { { new EmissionsForecast() { RequestedAt = DateTime.Now, GeneratedAt = DateTime.Now } } };
        var dataCollector = CreateForecastDataCollector(data);
        var response = await dataCollector.GetAsync("westus", DateTimeOffset.UtcNow.AddDays(1), 1, 1);

        Assert.IsNotNull(response);
        Assert.IsTrue(response.Equals(data.First()));
    }
}