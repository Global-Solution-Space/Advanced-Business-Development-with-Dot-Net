using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;
 
public record LocalizacaoRequest(
    [Range(-90.0, 90.0,   ErrorMessage = "Latitude deve estar entre -90 e 90.")]   decimal Latitude,
    [Range(-180.0, 180.0, ErrorMessage = "Longitude deve estar entre -180 e 180.")] decimal Longitude)
{
    public Localizacao ToDomain() => new(Latitude, Longitude);
}
