using TravelOrdersApp.Domain.Entities;

namespace TravelOrdersApp.Infrastructure.Repositories;

public interface ITravelOrderStateRepository
{
    public Task<List<TravelOrderState>> GetTravelOrderStateList();
}

public class TravelOrderStateRepository : ITravelOrderStateRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TravelOrderStateRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async  Task<List<TravelOrderState>> GetTravelOrderStateList()
    {
        await using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT Id
                                  ,Name
                              FROM TravelOrderState";

        var transports = new List<TravelOrderState>();
        using var reader = await cmd.ExecuteReaderAsync();
        transports = reader.MapToList<TravelOrderState>();
        return transports;
    }
}
