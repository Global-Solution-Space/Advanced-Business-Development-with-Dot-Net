using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record NasaPowerRequest(
    [Required][StringLength(8, MinimumLength = 8, ErrorMessage = "DataInicio deve estar no formato YYYYMMDD.")] string  DataInicio,
    [Required][StringLength(8, MinimumLength = 8, ErrorMessage = "DataFim deve estar no formato YYYYMMDD.")]    string  DataFim,
    [Range(-90.0, 90.0)]   decimal Latitude,
    [Range(-180.0, 180.0)] decimal Longitude,
    decimal                        Elevacao,
    [Required]             Guid    TalhaoId)
{
    public NasaPower ToDomain() => new(DataInicio, DataFim, Latitude, Longitude, Elevacao, TalhaoId);
}
