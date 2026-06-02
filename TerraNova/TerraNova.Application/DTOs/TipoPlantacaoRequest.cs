using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record TipoPlantacaoRequest(
    [Required][StringLength(30, MinimumLength = 2)] string TipoPlant)
{
    public TipoPlantacao ToDomain() => new(TipoPlant);
}
