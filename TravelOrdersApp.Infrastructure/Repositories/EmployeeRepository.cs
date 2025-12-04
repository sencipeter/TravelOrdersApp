using TravelOrdersApp.Domain.Entities;
using TravelOrdersApp.Domain.Requests;

namespace TravelOrdersApp.Infrastructure.Repositories;

public interface IEmployeeRepository
{
    public Task<List<Employee>> GetEmployeeList(EmployeeFilterListRequest? request = null);
}

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EmployeeRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async  Task<List<Employee>> GetEmployeeList(EmployeeFilterListRequest? request = null)
    {
        await using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync();

        //realne by to chcelo full text search alebo like @name + '%' skrz vyuzitia indexu
        //vyraz like '%' + @name + '%' sposobi full scan bez ohladu ci je index alebo nie je

        var cmd = conn.CreateCommand();
        cmd.CommandText = @$"SELECT Id
                              ,PersonalNumber
                              ,FirstName
                              ,LastName
                              ,DateOfBirth
                              ,PersonalIdentificationNumber
                          FROM Employee
                          where 1=1 
                          {(!string.IsNullOrEmpty(request?.Name) ? "and FirstName + ' ' +  LastName like '%' + @name + '%' " : "")}";

        if (!string.IsNullOrEmpty(request?.Name))
            cmd.Parameters.AddWithValue("@name", request?.Name);


        var cities = new List<Employee>();
        using var reader = await cmd.ExecuteReaderAsync();
        cities = reader.MapToList<Employee>();
        return cities;
    }
}
