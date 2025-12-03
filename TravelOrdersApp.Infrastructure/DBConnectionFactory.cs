namespace TravelOrdersApp.Infrastructure;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public interface IDbConnectionFactory
{
    SqlConnection CreateConnection();
}

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {        
        _connectionString = configuration.GetConnectionString("AppConnectionString")
            ?? throw new InvalidOperationException("Connection string 'AppConnectionString' not found.");
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}