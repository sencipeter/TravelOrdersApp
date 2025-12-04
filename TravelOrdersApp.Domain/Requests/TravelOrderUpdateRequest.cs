using System.ComponentModel.DataAnnotations;

namespace TravelOrdersApp.Domain.Requests;

public class TravelOrderUpdateRequest: TravelOrderAddRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0.")]
    public int? Id { get; set; }
}
