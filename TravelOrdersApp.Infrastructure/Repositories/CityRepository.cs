using TravelOrdersApp.Domain.Entities;

namespace TravelOrdersApp.Infrastructure.Repositories;

public interface ICityRepository
{
    public Task<List<City>> GetCityList();
}

public class CityRepository : ICityRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CityRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async  Task<List<City>> GetCityList()
    {
        await using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT Id
                                  ,CityName
                                  ,Country
                                  ,Location
                              FROM City";

        var cities = new List<City>();
        using var reader = await cmd.ExecuteReaderAsync();
        cities = reader.MapToList<City>();
        return cities;
    }
}
