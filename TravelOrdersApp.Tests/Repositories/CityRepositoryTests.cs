using Microsoft.Extensions.Configuration;
using TravelOrdersApp.Infrastructure;
using TravelOrdersApp.Infrastructure.Repositories;
using Xunit;

namespace TravelOrdersApp.Tests.Repositories;

public class CityRepositoryTests
{
    ICityRepository _cityRepository; 
    IDbConnectionFactory _connectionFactory;

    public CityRepositoryTests()
    {
        // Build configuration from appsettings.json
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        _connectionFactory = new SqlConnectionFactory(config);
        _cityRepository = new CityRepository(_connectionFactory);
    }

    [Fact]
    public async Task GetCitiestest()
    {
        var result = await _cityRepository.GetCityList();
        Assert.NotNull(result);
        Assert.True(result.Any());

    }
}
