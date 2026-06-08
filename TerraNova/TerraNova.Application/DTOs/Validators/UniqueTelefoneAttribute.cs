using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
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
        if (telefoneRepo == null) return ValidationResult.Success;

        // Tenta obter o ID da rota para cenários de UPDATE.
        var httpContextAccessor = (IHttpContextAccessor?)validationContext.GetService(typeof(IHttpContextAccessor));
        var routeId = httpContextAccessor?.HttpContext?.Request.RouteValues["id"]?.ToString();
        Guid.TryParse(routeId, out var currentId);
        bool isUpdate = currentId != Guid.Empty;

        string? ddd = null;
        string? numero = null;

        if (value is string telefoneLimpoStr)
        {
            // Usado em ProdutorRequest.TelefoneContato (string com DDD + Número)
            var telefoneFiltro = new string(telefoneLimpoStr.Where(char.IsDigit).ToArray());
            if (telefoneFiltro.Length is 10 or 11)
            {
                ddd = telefoneFiltro.Substring(0, 2);
                numero = telefoneFiltro.Substring(2);
            }
        }
        else if (value is TelefoneRequest request)
        {
            // DTO de telefone detalhado (usado em Create e Update)
            ddd = request.Ddd;
            numero = request.Numero;
        }

        if (ddd is null || numero is null)
            return ValidationResult.Success;

        if (isUpdate)
        {
            // UPDATE: ignora o telefone do proprio produtor (relacao 1:1).
            if (telefoneRepo.GetAll().Any(t => t.Ddd == ddd && t.Numero == numero && t.ProdutorId != currentId))
                return new ValidationResult("Este DDD e Número já estão cadastrados para outro telefone.");
        }
        else
        {
            // CREATE: qualquer telefone com o mesmo DDD e Número é conflito.
            if (telefoneRepo.GetAll().Any(t => t.Ddd == ddd && t.Numero == numero))
                return new ValidationResult("Este DDD e Número já estão cadastrados.");
        }

        return ValidationResult.Success;
    }
}