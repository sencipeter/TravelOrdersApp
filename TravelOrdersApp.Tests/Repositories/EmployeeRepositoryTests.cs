using Microsoft.Extensions.Configuration;
using TravelOrdersApp.Domain.Requests;
using TravelOrdersApp.Infrastructure;
using TravelOrdersApp.Infrastructure.Repositories;
using Xunit;

namespace TravelOrdersApp.Tests.Repositories;

public class EmployeeRepositoryTests
{
    IEmployeeRepository _employeeRepository; 
    IDbConnectionFactory _connectionFactory;

    public EmployeeRepositoryTests()
    {
        // Build configuration from appsettings.json
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        _connectionFactory = new SqlConnectionFactory(config);
        _employeeRepository = new EmployeeRepository(_connectionFactory);
    }

    [Fact]
    public async Task GetAllEmployeeTest()
    {
        var result = await _employeeRepository.GetEmployeeList();
        Assert.NotNull(result);
        Assert.True(result.Count > 1);

    }

    [Fact]
    public async Task GetEmployeeFilterByNameTest()
    {
        var result = await _employeeRepository.GetEmployeeList(
            new EmployeeFilterListRequest 
            { 
                Name = "horva" 
            });

        Assert.NotNull(result);
        Assert.Single(result);
    }
}
