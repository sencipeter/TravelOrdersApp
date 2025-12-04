using Microsoft.Extensions.Configuration;
using TravelOrdersApp.Infrastructure;
using TravelOrdersApp.Infrastructure.Repositories;
using Xunit;

namespace TravelOrdersApp.Tests.Repositories;

public class TravelOrderStateRepositoryTests
{
    ITravelOrderStateRepository _travelOrderStateRepository; 
    IDbConnectionFactory _connectionFactory;

    public TravelOrderStateRepositoryTests()
    {
        // Build configuration from appsettings.json
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        _connectionFactory = new SqlConnectionFactory(config);
        _travelOrderStateRepository = new TravelOrderStateRepository(_connectionFactory);
    }

    [Fact]
    public async Task GetTravelOrderStateListTests()
    {
        var result = await _travelOrderStateRepository.GetTravelOrderStateList();
        Assert.NotNull(result);
        Assert.True(result.Any());

    }
}
