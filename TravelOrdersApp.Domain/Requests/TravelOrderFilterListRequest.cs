namespace TravelOrdersApp.Domain.Requests;

public class TravelOrderFilterListRequest
{
    public int? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
}
