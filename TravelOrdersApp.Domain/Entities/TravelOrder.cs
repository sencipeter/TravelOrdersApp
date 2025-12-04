using System.ComponentModel.DataAnnotations;

namespace TravelOrdersApp.Domain.Entities;

public class TravelOrder:BaseEntity
{
    public DateTime CreatedDate { get; set; }

    [Required]
    public int? EmployeeId { get; set; }

    [Required]
    public int? StartingLocationCityId { get; set; }

    [Required]
    public int? DestinationCityId { get; set; }

    [Required]
    public DateTime? BusinessTripStart { get; set; }

    [Required]
    public DateTime? BusinessTripEnd { get; set; }
    
    [Required]
    public int? TravelOrderStateId { get; set; }

    // Navigation properties
    public Employee? Employee { get; set; }
    public City? StartingLocationCity { get; set; }
    public City? DestinationCity { get; set; }
    public TravelOrderState? TravelOrderState { get; set; }
    public Transport? Transport { get; set; }
    public List<Transport> Transports { get; set; } = new List<Transport>();
}
