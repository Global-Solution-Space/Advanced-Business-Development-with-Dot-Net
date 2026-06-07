using System.ComponentModel.DataAnnotations;
using TerraNova.Application.Repositories;
using TerraNova.Application.DTOs;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Parameter)]
public sealed class UniqueTelefoneAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var telefoneRepo = (IRepository<Telefone>?)validationContext.GetService(typeof(IRepository<Telefone>));

        if (value is string telefoneLimpoStr)
        {
            var telefoneFiltro = new string(telefoneLimpoStr.Where(char.IsDigit).ToArray());
            if (telefoneFiltro.Length is 10 or 11)
            {
                var ddd = telefoneFiltro.Substring(0, 2);
                var num = telefoneFiltro.Substring(2);
                if (telefoneRepo != null && telefoneRepo.GetAll().Any(t => t.Ddd == ddd && t.Numero == num))
                    return new ValidationResult("Este telefone principal já está cadastrado em outro produtor.");
            }
        }
        else if (value is TelefoneRequest request)
        {
            // Validando o DTO inteiro de TelefoneRequest (Ddd + Numero)
            if (telefoneRepo != null && telefoneRepo.GetAll().Any(t => t.Ddd == request.Ddd && t.Numero == request.Numero))
                return new ValidationResult("Este DDD e Número já estão cadastrados.");
        }

        return ValidationResult.Success;
    }
}
