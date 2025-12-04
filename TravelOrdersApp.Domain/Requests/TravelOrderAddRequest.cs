using System.ComponentModel.DataAnnotations;
using TravelOrdersApp.Domain.Attributes;

namespace TravelOrdersApp.Domain.Requests;

public class TravelOrderAddRequest: IValidatableObject
{
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

    [AtLeastOneItem]
    public List<int> TransportIdList { get; set; } = new List<int>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BusinessTripStart.HasValue && BusinessTripEnd.HasValue)
        {
            if (BusinessTripEnd.Value < BusinessTripStart.Value)
            {
                yield return new ValidationResult(
                    "Business Trip End must be greater than or equal to Business Trip Start.",
                    new[] { nameof(BusinessTripEnd) });
            }
        }
    }
}
