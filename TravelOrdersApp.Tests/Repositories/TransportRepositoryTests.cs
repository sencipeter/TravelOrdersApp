using Microsoft.Extensions.Configuration;
using TravelOrdersApp.Infrastructure;
using TravelOrdersApp.Infrastructure.Repositories;
using Xunit;

namespace TravelOrdersApp.Tests.Repositories;

public class TransportRepositoryTests
{
    ITransportRepository _transportRepository; 
    IDbConnectionFactory _connectionFactory;

    public TransportRepositoryTests()
    {
        // Build configuration from appsettings.json
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        _connectionFactory = new SqlConnectionFactory(config);
        _transportRepository = new TransportRepository(_connectionFactory);
    }

    [Fact]
    public async Task GetTransportListTests()
    {
        var result = await _transportRepository.GetTransportList();
        Assert.NotNull(result);
        Assert.True(result.Any());

    }
}
