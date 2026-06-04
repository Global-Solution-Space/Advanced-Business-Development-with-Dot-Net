using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

public record TipoApiRequest(
    [Required][StringLength(10, MinimumLength = 2)] string NomeTipoApi)
{
    public TipoApi ToDomain() => new(NomeTipoApi);
}
