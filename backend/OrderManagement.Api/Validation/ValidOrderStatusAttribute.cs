using System.ComponentModel.DataAnnotations;
using OrderManagement.Api.Constants;

namespace OrderManagement.Api.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class ValidOrderStatusAttribute : ValidationAttribute
{
    public ValidOrderStatusAttribute()
        : base("Status invalid. Valori permise: Pending, Processing, Completed, Cancelled.")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string status && OrderStatus.IsValid(status))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(
            $"Status invalid. Valori permise: {string.Join(", ", OrderStatus.All)}.");
    }
}
