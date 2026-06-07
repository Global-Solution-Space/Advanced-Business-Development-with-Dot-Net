using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class DadoTemporalController(IDadoTemporalService dadoTemporalService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DadoTemporalResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(dadoTemporalService.GetAll());

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DadoTemporalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var d = dadoTemporalService.GetById(id);
        return d is null ? NotFound() : Ok(d);
    }

    [HttpGet("talhao/{talhaoId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<DadoTemporalResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByTalhaoId(Guid talhaoId) =>
        Ok(dadoTemporalService.GetByTalhaoId(talhaoId));

    [HttpGet("req-api/{reqApiId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<DadoTemporalResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByReqApiId(Guid reqApiId) =>
        Ok(dadoTemporalService.GetByReqApiId(reqApiId));
}
