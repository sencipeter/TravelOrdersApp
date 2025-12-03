using Microsoft.Extensions.Configuration;
using TravelOrdersApp.Infrastructure;
using TravelOrdersApp.Infrastructure.Repositories;
using Xunit;

namespace TravelOrdersApp.Tests.Repositories;

public class TravelOrderRepositoryTests
{
    ITravelOrderRepository _travelOrderRepository; 
    IDbConnectionFactory _connectionFactory;

    public TravelOrderRepositoryTests()
    {
        // Build configuration from appsettings.json
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        _connectionFactory = new SqlConnectionFactory(config);
        _travelOrderRepository = new TravelOrderRepository(_connectionFactory);
    }

    [Fact]
    public async Task GetTravelOrderListFilterByIdTests()
    {
        var result = await _travelOrderRepository.GetTravelOrderList(employeeId: 2);
        Assert.NotNull(result);
        Assert.Single(result);

    }

    [Fact]
    public async Task GetTravelOrderListFilterByNameTests()
    {
        var result = await _travelOrderRepository.GetTravelOrderList(employeeName: "jan nov");
        Assert.NotNull(result);
        Assert.Single(result);

    }

    [Fact]
    public async Task GetTravelOrderListFilterByIdAndNameTests()
    {
        var result = await _travelOrderRepository.GetTravelOrderList(employeeId: 2, employeeName: "pet");
        Assert.NotNull(result);
        Assert.Single(result);

    }

    [Fact]
    public async Task GetTravelOrderListAllTests()
    {
        var result = await _travelOrderRepository.GetTravelOrderList();
        Assert.NotNull(result);
        Assert.True(result.Count > 1);

    }
}
