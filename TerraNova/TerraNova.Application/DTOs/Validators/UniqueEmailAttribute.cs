using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
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

        // Tenta obter o ID da rota para cenários de UPDATE.
        var httpContextAccessor = (IHttpContextAccessor?)validationContext.GetService(typeof(IHttpContextAccessor));
        var routeId = httpContextAccessor?.HttpContext?.Request.RouteValues["id"]?.ToString();

        if (Guid.TryParse(routeId, out var currentId))
        {
            // Ignora o próprio produtor ao verificar unicidade.
            if (repository.GetAll().Any(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && p.Id != currentId))
            {
                return new ValidationResult("Este e-mail já está em uso por outro produtor.");
            }
        }
        else
        {
            if (repository.GetAll().Any(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                return new ValidationResult("Este e-mail já está em uso.");
            }
        }

        return ValidationResult.Success;
    }
}