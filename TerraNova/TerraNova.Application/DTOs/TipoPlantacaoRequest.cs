using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

/// <summary>Dados para cadastrar um tipo de plantação.</summary>
/// <param name="TipoPlant">Nome do tipo de plantação.</param>
public record TipoPlantacaoRequest(
    [Required][StringLength(30, MinimumLength = 2)] string TipoPlant)
{
    public TipoPlantacao ToDomain() => new(TipoPlant);
}
