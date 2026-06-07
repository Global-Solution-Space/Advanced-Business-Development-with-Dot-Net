using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Enums;

namespace TerraNova.API.Controllers;

/// <summary>
/// Requisições a APIs externas (NASA POWER e Embrapa SatVeg).
/// Executa integrações assíncronas e persiste os dados temporais resultantes.
/// </summary>
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

    /// <summary>
    /// Lista todas as requisições que possuem dados para um talhão específico.
    /// Utiliza EXISTS/INNER JOIN otimizado via LINQ .Any().
    /// </summary>
    [HttpGet("talhao/{talhaoId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<ReqApiResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByTalhaoId(Guid talhaoId) =>
        Ok(reqApiService.GetByTalhaoId(talhaoId));

    /// <summary>
    /// Executa uma nova integração externa e persiste os dados temporais.
    /// 
    /// **TipoParam**: 0 = NDVI (SatVeg/Embrapa), 1 = PRECTOTCORR (NASA POWER)
    /// 
    /// A coordenada é extraída automaticamente do Talhão associado.
    /// Para NASA POWER, datas são fixadas de 2020-01-01 até hoje.
    /// Valores -900.0 (falha de sensor) são ignorados.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ReqApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ReqApiRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await reqApiService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Remove uma requisição e seus dados temporais associados (cascade).</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        reqApiService.Delete(id) ? NoContent() : NotFound();
}