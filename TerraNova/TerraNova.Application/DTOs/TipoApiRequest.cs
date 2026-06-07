using System.ComponentModel.DataAnnotations;
using TerraNova.Domain.Entities;

namespace TerraNova.Application.DTOs;

/// <summary>Dados para cadastrar um tipo de API externa.</summary>
/// <param name="NomeTipoApi">Nome do tipo de API, como SATVEG ou NASAPOWER.</param>
public record TipoApiRequest(
    [Required][StringLength(10, MinimumLength = 2)] string NomeTipoApi)
{
    public TipoApi ToDomain() => new(NomeTipoApi);
}
