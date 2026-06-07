using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.API.Controllers;

/// <summary>Orquestrador de dados de APIs externas (NASA POWER / SatVeg).</summary>
[Tags("Integração (Req API)")]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ReqApiController(IReqApiService reqApiService) : ControllerBase
{
    /// <summary>Lista todas as requisições de API realizadas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ReqApiResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(reqApiService.GetAll());

    /// <summary>Obtém uma requisição pelo ID, incluindo a contagem de dados salvos.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReqApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var r = reqApiService.GetById(id);
        return r is null ? NotFound() : Ok(r);
    }

    /// <summary>Buscar requisições associadas a um talhão específico.</summary>
    [HttpGet("talhao/{talhaoId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<ReqApiResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByTalhaoId(Guid talhaoId) =>
        Ok(reqApiService.GetByTalhaoId(talhaoId));

    /// <summary>Cadastrar</summary>
    /// <remarks>
    /// tipoParam: 0 = NDVI (SatVeg/Embrapa), 1 = PRECTOTCORR (NASA POWER).
    /// A coordenada é extraída automaticamente do talhão associado.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ReqApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ReqApiRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await reqApiService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Remove uma requisição e seus dados temporais associados</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        reqApiService.Delete(id) ? NoContent() : NotFound();
}