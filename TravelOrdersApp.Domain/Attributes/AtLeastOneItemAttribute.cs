using System.ComponentModel.DataAnnotations;

namespace TravelOrdersApp.Domain.Attributes;

public class AtLeastOneItemAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is ICollection<int> list && list.Count > 0)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult($"{validationContext.DisplayName} must contain at least one item.");
    }
}
