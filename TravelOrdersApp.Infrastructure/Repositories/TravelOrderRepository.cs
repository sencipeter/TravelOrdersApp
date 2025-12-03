using TravelOrdersApp.Domain.Entities;

namespace TravelOrdersApp.Infrastructure.Repositories;

public interface ITravelOrderRepository
{
    public Task<List<TravelOrder>> GetTravelOrderList(string? employeeName = null,
        int? employeeId = null);
}

public class TravelOrderRepository : ITravelOrderRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TravelOrderRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async  Task<List<TravelOrder>> GetTravelOrderList(string? employeeName = null, 
        int? employeeId = null)
    {
        await using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @$"SELECT TravelOrder.Id
                                  ,CreatedDate
                                  ,EmployeeId
                                  ,Employee.FirstName as [Employee.FirstName]
                                  ,Employee.LastName as [Employee.LastName]
                                  ,StartingLocationCityId
                                  ,StartingLocationCity.CityName as [StartingLocationCity.CityName]
                                  ,DestinationCityId
                                  ,DestinationCity.CityName as [DestinationCity.CityName]
                                  ,BusinessTripStart
                                  ,BusinessTripEnd
                                  ,TravelOrderStateId
                                  ,TravelOrderState.Name as [TravelOrderState.Name]
                              FROM TravelOrder
                              LEFT JOIN Employee on Employee.Id = TravelOrder.EmployeeId
                              LEFT JOIN City as StartingLocationCity on StartingLocationCity.Id = TravelOrder.StartingLocationCityId
                              LEFT JOIN City as DestinationCity on DestinationCity.Id = TravelOrder.DestinationCityId
                              LEFT JOIN TravelOrderState on TravelOrderState.Id = TravelOrder.TravelOrderStateId
                              where 1=1 
                                {(!string.IsNullOrEmpty(employeeName) ? "and Employee.FirstName + ' ' +  Employee.LastName like '%' + @employeeName + '%' " : "")}
                                {(employeeId is not null ? "and Employee.Id = @employeeId " : "")}";

        if (!string.IsNullOrEmpty(employeeName))
            cmd.Parameters.AddWithValue("@employeeName", employeeName);

        if (employeeId is not null)
            cmd.Parameters.AddWithValue("@employeeId", employeeId);

        var travelOrders = new List<TravelOrder>();
        using var reader = await cmd.ExecuteReaderAsync();
        travelOrders = reader.MapToList<TravelOrder>();
        return travelOrders;
    }
}
