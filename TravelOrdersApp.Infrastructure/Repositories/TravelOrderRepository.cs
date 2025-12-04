using Microsoft.Data.SqlClient;
using TravelOrdersApp.Domain.Entities;
using TravelOrdersApp.Domain.Requests;

namespace TravelOrdersApp.Infrastructure.Repositories;

public interface ITravelOrderRepository
{
    public Task<List<TravelOrder>> GetTravelOrderList(TravelOrderFilterListRequest? request = null);
    Task<TravelOrder?> GetTravelOrder(int id);
    public Task<TravelOrder> Add(TravelOrderAddRequest request);
    public Task<TravelOrder> Update(TravelOrderUpdateRequest request);

    /// <summary>
    /// Delete Travel Order by Id 
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Count affected records</returns>
    public Task<int> Delete(int id);
}

public class TravelOrderRepository : ITravelOrderRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TravelOrderRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    public async Task<TravelOrder> Add(TravelOrderAddRequest request)
    {
        var travelOrder = new TravelOrder();

        using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync();

        using var transaction = (SqlTransaction)(await conn.BeginTransactionAsync());

        try
        {

            using var cmd = new SqlCommand(@"
            INSERT INTO TravelOrder
                (EmployeeId, StartingLocationCityId, DestinationCityId,
                 BusinessTripStart, BusinessTripEnd, TravelOrderStateId)
            OUTPUT INSERTED.Id
            VALUES (@EmployeeId, @StartingLocationCityId, @DestinationCityId,
                    @BusinessTripStart, @BusinessTripEnd, @TravelOrderStateId)", conn, transaction);

            cmd.AddParametersFromObject(request);

            travelOrder.Id = (int)(await cmd.ExecuteScalarAsync());

            // Insert related transports
            foreach (var transportId in request.TransportIdList)
            {
                using var cmdTransport = new SqlCommand(@"
                    INSERT INTO TravelOrderTransport (TransportId, TravelOrderId)
                    VALUES (@TransportId, @TravelOrderId)", conn, transaction);

                cmdTransport.Parameters.AddWithValue("@TransportId", transportId);
                cmdTransport.Parameters.AddWithValue("@TravelOrderId", travelOrder.Id);

                cmdTransport.ExecuteNonQuery();
            }

            await transaction.CommitAsync();

            travelOrder = await GetTravelOrder(travelOrder.Id);

            return travelOrder;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

    }

    public async Task<TravelOrder> Update(TravelOrderUpdateRequest request)
    {

        using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync();

        using var transaction = (SqlTransaction)(await conn.BeginTransactionAsync());

        try
        {

            using var cmd = new SqlCommand(@"UPDATE TravelOrder
                                                SET EmployeeId = @EmployeeId,
                                                    StartingLocationCityId = @StartingLocationCityId,
                                                    DestinationCityId = @DestinationCityId,
                                                    BusinessTripStart = @BusinessTripStart,
                                                    BusinessTripEnd = @BusinessTripEnd,
                                                    TravelOrderStateId = @TravelOrderStateId
                                                WHERE Id = @Id;", conn, transaction);

            cmd.AddParametersFromObject(request);

            await cmd.ExecuteNonQueryAsync();

            using var cmdDeleteTransport = new SqlCommand(@"DELETE FROM TravelOrderTransport WHERE TravelOrderId = @Id;",
                conn,
                transaction);
            cmdDeleteTransport.Parameters.AddWithValue("@Id", request.Id);
            await cmdDeleteTransport.ExecuteNonQueryAsync();

            // Insert related transports
            foreach (var transportId in request.TransportIdList)
            {
                using var cmdTransport = new SqlCommand(@"
                    INSERT INTO TravelOrderTransport (TransportId, TravelOrderId)
                    VALUES (@TransportId, @TravelOrderId)", conn, transaction);

                cmdTransport.Parameters.AddWithValue("@TransportId", transportId);
                cmdTransport.Parameters.AddWithValue("@TravelOrderId", request.Id);

                cmdTransport.ExecuteNonQuery();
            }

            await transaction.CommitAsync();

            var travelOrder = await GetTravelOrder(request.Id.Value);
            return travelOrder;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

    }

    public async Task<int> Delete(int id)
    {
        var result = 0;
        
        using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync();

        using var transaction = (SqlTransaction)(await conn.BeginTransactionAsync());

        try
        {

            using var cmdTransport = new SqlCommand(@"DELETE FROM TravelOrderTransport WHERE TravelOrderId = @Id;",
                conn,
                transaction);
            cmdTransport.Parameters.AddWithValue("@Id", id);
            await cmdTransport.ExecuteNonQueryAsync();

            using var cmd = new SqlCommand(@"DELETE FROM TravelOrder WHERE Id = @Id;",
                conn,
                transaction);
            cmd.Parameters.AddWithValue("@Id", id);

            result = cmd.ExecuteNonQuery();

            await transaction.CommitAsync();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

    }

    public async Task<TravelOrder?> GetTravelOrder(int id)
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
                                  ,Transport.Id as [Transport.Id]
                                  ,Transport.Name as [Transport.Name]
                              FROM TravelOrder
                              LEFT JOIN Employee on Employee.Id = TravelOrder.EmployeeId
                              LEFT JOIN City as StartingLocationCity on StartingLocationCity.Id = TravelOrder.StartingLocationCityId
                              LEFT JOIN City as DestinationCity on DestinationCity.Id = TravelOrder.DestinationCityId
                              LEFT JOIN TravelOrderState on TravelOrderState.Id = TravelOrder.TravelOrderStateId
                              LEFT JOIN TravelOrderTransport on TravelOrderTransport.TravelOrderId = TravelOrder.Id
                              LEFT JOIN Transport on Transport.Id = TravelOrderTransport.TransportId
                              where TravelOrder.Id = @Id";

        cmd.Parameters.AddWithValue("@Id", id);

        var travelOrders = new List<TravelOrder>();
        using var reader = await cmd.ExecuteReaderAsync();
        travelOrders = reader.MapToList<TravelOrder>();


        TravelOrder? travelOrder = travelOrders
            .FirstOrDefault();

        travelOrder?.Transports = travelOrders?
            .Where(x => x.Transport is not null)
            .Select(x => x.Transport)?
            .ToList();
        return travelOrder;
    }

    public async Task<List<TravelOrder>> GetTravelOrderList(TravelOrderFilterListRequest? request = null)
    {
        await using var conn = _connectionFactory.CreateConnection();
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @$"SELECT TravelOrder.Id
                                  ,CreatedDate
                                  ,EmployeeId
                                  ,Employee.FirstName as [Employee.FirstName]
                                  ,Employee.LastName as [Employee.LastName]
                                  ,Employee.PersonalNumber as [Employee.PersonalNumber]
                                  ,Employee.DateOfBirth as [Employee.DateOfBirth]
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
                                {(!string.IsNullOrEmpty(request?.EmployeeName) ? "and Employee.FirstName + ' ' +  Employee.LastName like '%' + @EmployeeName + '%' " : "")}
                                {(request?.EmployeeId is not null ? "and Employee.Id = @EmployeeId " : "")}";
        if (request is not null)
            cmd.AddParametersFromObject(request);

        var travelOrders = new List<TravelOrder>();
        using var reader = await cmd.ExecuteReaderAsync();
        travelOrders = reader.MapToList<TravelOrder>();
        return travelOrders;
    }
}
