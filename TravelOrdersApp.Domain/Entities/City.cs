using Microsoft.SqlServer.Types;

namespace TravelOrdersApp.Domain.Entities;

public class City: BaseEntity
{
    public string CityName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public SqlGeography? Location { get; set; }
}
