using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.API.Controllers;

/// <summary>
/// Dados temporais (séries históricas) de NDVI e precipitação.
/// Cada registro representa uma leitura de um sensor externo para um talhão em uma data específica.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class DadoTemporalController(IDadoTemporalService dadoTemporalService) : ControllerBase
{
    /// <summary>Lista todos os dados temporais cadastrados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DadoTemporalResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(dadoTemporalService.GetAll());

    /// <summary>Obtém um dado temporal pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DadoTemporalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var d = dadoTemporalService.GetById(id);
        return d is null ? NotFound() : Ok(d);
    }

    /// <summary>Lista todos os dados temporais de um talhão, ordenados por data de leitura.</summary>
    [HttpGet("talhao/{talhaoId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<DadoTemporalResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByTalhaoId(Guid talhaoId) =>
        Ok(dadoTemporalService.GetByTalhaoId(talhaoId));

    /// <summary>Lista todos os dados temporais de uma requisição de API específica.</summary>
    [HttpGet("req-api/{reqApiId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<DadoTemporalResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByReqApiId(Guid reqApiId) =>
        Ok(dadoTemporalService.GetByReqApiId(reqApiId));
}