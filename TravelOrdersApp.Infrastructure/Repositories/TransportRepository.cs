using TravelOrdersApp.Domain.Entities;

namespace TravelOrdersApp.Infrastructure.Repositories;

public interface ITransportRepository
{
    public Task<List<Transport>> GetTransportList();
}

public class TransportRepository : ITransportRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TransportRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async  Task<List<Transport>> GetTransportList()
    {
        await using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT Id
                                  ,Name
                              FROM Transport";

        var transports = new List<Transport>();
        using var reader = await cmd.ExecuteReaderAsync();
        transports = reader.MapToList<Transport>();
        return transports;
    }
}
