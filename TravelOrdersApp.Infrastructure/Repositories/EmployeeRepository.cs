using TravelOrdersApp.Domain.Entities;

namespace TravelOrdersApp.Infrastructure.Repositories;

public interface IEmployeeRepository
{
    public Task<List<Employee>> GetEmployeeList(string? name = null);
}

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EmployeeRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async  Task<List<Employee>> GetEmployeeList(string? name = null)
    {
        await using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @$"SELECT Id
                              ,PersonalNumber
                              ,FirstName
                              ,LastName
                              ,DateOfBirth
                              ,PersonalIdentificationNumber
                          FROM Employee
                          where 1=1 
                          {(!string.IsNullOrEmpty(name) ? "and FirstName + ' ' +  LastName like '%' + @name + '%' " : "")}";

        if (!string.IsNullOrEmpty(name))
            cmd.Parameters.AddWithValue("@name", name);


        var cities = new List<Employee>();
        using var reader = await cmd.ExecuteReaderAsync();
        cities = reader.MapToList<Employee>();
        return cities;
    }
}
