using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using TravelOrdersApp.Domain.Requests;
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
        var result = await _travelOrderRepository.GetTravelOrderList(
            new TravelOrderFilterListRequest 
            { 
                EmployeeId = 2 
            });

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task CreateTravelOrderTests()
    {
        var result = await _travelOrderRepository.Add(
            new TravelOrderAddRequest
            {
                EmployeeId = 2,
                BusinessTripEnd = DateTime.Now.AddMonths(1),
                BusinessTripStart = DateTime.Now,
                DestinationCityId = 5,
                StartingLocationCityId = 2,
                TransportIdList = new List<int> { 1, 3, 6 },
                TravelOrderStateId = 2
            });

        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal(3, result.Transports.Count);
    }

    [Fact]
    public async Task DeleteTravelOrderTests()
    {
        var order = await _travelOrderRepository.Add(
            new TravelOrderAddRequest
            {
                EmployeeId = 3,
                BusinessTripEnd = DateTime.Now.AddMonths(3),
                BusinessTripStart = DateTime.Now.AddMonths(2),
                DestinationCityId = 5,
                StartingLocationCityId = 2,
                TransportIdList = new List<int> { 1, 2, 5, 7 },
                TravelOrderStateId = 3
            });

        Assert.NotNull(order);
        Assert.NotEqual(0, order.Id);

        var result = await _travelOrderRepository.Delete(order.Id);
        Assert.Equal(1, result);

        order = await _travelOrderRepository.GetTravelOrder(order.Id);
        Assert.Null(order);
    }

    [Fact]
    public async Task UpdateTravelOrderTests()
    {
        var requestInsert = new TravelOrderAddRequest
        {
            EmployeeId = 4,
            BusinessTripEnd = DateTime.Now.AddMonths(3),
            BusinessTripStart = DateTime.Now.AddMonths(2),
            DestinationCityId = 5,
            StartingLocationCityId = 2,
            TransportIdList = new List<int> { 1, 2, 5, 7 },
            TravelOrderStateId = 3
        };

        var requestUpdate = new TravelOrderUpdateRequest
        {
            EmployeeId = 5,
            BusinessTripEnd = DateTime.Now.AddMonths(5),
            BusinessTripStart = DateTime.Now.AddMonths(4),
            DestinationCityId = 1,
            StartingLocationCityId = 6,
            TransportIdList = new List<int> { 3, 4 },
            TravelOrderStateId = 4
        };

        var order = await _travelOrderRepository.Add(requestInsert);

        Assert.NotNull(order);
        Assert.NotEqual(0, order.Id);

        requestUpdate.Id = order.Id;

        var result = await _travelOrderRepository.Update(requestUpdate);
        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
        Assert.Equal(requestUpdate.EmployeeId, result.EmployeeId);
        Assert.Equal(requestUpdate.BusinessTripEnd.Value.ToString("dd. MM. yyyy HH:mm"), result.BusinessTripEnd.Value.ToString("dd. MM. yyyy HH:mm"));
        Assert.Equal(requestUpdate.BusinessTripStart.Value.ToString("dd. MM. yyyy HH:mm"), result.BusinessTripStart.Value.ToString("dd. MM. yyyy HH:mm"));
        Assert.Equal(requestUpdate.DestinationCityId, result.DestinationCityId);
        Assert.Equal(requestUpdate.TravelOrderStateId, result.TravelOrderStateId);
        Assert.Equal(requestUpdate.TransportIdList.Count, result.Transports.Count);
        Assert.Equal(requestUpdate.EmployeeId, result.EmployeeId);

    }

    [Fact]
    public async Task GetTravelOrderByIdTests()
    {
        var result = await _travelOrderRepository.GetTravelOrder(id: 2);

        Assert.NotNull(result);
        Assert.True(result.Transports.Any());
    }

    [Fact]
    public async Task GetTravelOrderListFilterByNameTests()
    {
        var result = await _travelOrderRepository.GetTravelOrderList(
            new TravelOrderFilterListRequest
            {
                EmployeeName = "jan nov"
            });

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetTravelOrderListFilterByIdAndNameTests()
    {
        var result = await _travelOrderRepository.GetTravelOrderList(
            new TravelOrderFilterListRequest
            {
                EmployeeName = "pet",
                EmployeeId =2
            });
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

    [Fact]
    public async Task GetTravelEmptyOrderListTests()
    {
        var result = await _travelOrderRepository.GetTravelOrderList(
            new TravelOrderFilterListRequest
            {
                EmployeeName = "pe----t",
                EmployeeId = 20
            });
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
