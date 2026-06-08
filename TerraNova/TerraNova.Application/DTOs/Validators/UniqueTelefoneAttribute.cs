using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using TerraNova.Application.DTOs;
using TerraNova.Application.Repositories;

namespace TerraNova.Application.DTOs.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Parameter)]
public sealed class UniqueTelefoneAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var telefoneRepo = (ITelefoneRepository?)validationContext.GetService(typeof(ITelefoneRepository));
        if (telefoneRepo is null) return ValidationResult.Success;

        var httpContextAccessor = (IHttpContextAccessor?)validationContext.GetService(typeof(IHttpContextAccessor));
        var routeId = httpContextAccessor?.HttpContext?.Request.RouteValues["id"]?.ToString();
        var isUpdate = Guid.TryParse(routeId, out var currentId) && currentId != Guid.Empty;

        var parsed = ExtrairTelefone(value);
        if (parsed is null) return ValidationResult.Success;

        var (ddd, numero) = parsed.Value;

        if (isUpdate && value is string)
        {
            if (telefoneRepo.ExistsByDddNumeroExceptProdutorId(ddd, numero, currentId))
                return new ValidationResult("Este DDD e número já estão cadastrados para outro telefone.");
        }
        else if (isUpdate && value is TelefoneRequest)
        {
            if (telefoneRepo.ExistsByDddNumeroExceptId(ddd, numero, currentId))
                return new ValidationResult("Este DDD e número já estão cadastrados para outro telefone.");
        }
        else if (telefoneRepo.ExistsByDddNumero(ddd, numero))
        {
            return new ValidationResult("Este DDD e número já estão cadastrados.");
        }

        return ValidationResult.Success;
    }

    private static (string Ddd, string Numero)? ExtrairTelefone(object? value)
    {
        if (value is string telefoneContato)
        {
            var digits = new string(telefoneContato.Where(char.IsDigit).ToArray());
            return digits.Length is 10 or 11
                ? (digits[..2], digits[2..])
                : null;
        }

        if (value is TelefoneRequest request)
            return (request.Ddd, request.Numero);

        return null;
    }
}
