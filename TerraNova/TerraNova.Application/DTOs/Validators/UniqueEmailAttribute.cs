using System.ComponentModel.DataAnnotations;
using TerraNova.Application.Repositories;

namespace TerraNova.Application.DTOs.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string email) return ValidationResult.Success;

        var repository = (IProdutorRepository?)validationContext.GetService(typeof(IProdutorRepository));
        if (repository == null) return ValidationResult.Success;

        if (repository.GetAll().Any(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            return new ValidationResult("Este e-mail já está em uso.");
        }

        return ValidationResult.Success;
    }
}
