using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.API.Controllers;

/// <summary>Dados climáticos NASA POWER. Vinculados a um Talhão.</summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class NasaPowerController(INasaPowerService nasaPowerService) : ControllerBase
{
    /// <summary>Lista todos os registros NASA POWER cadastrados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<NasaPowerResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(nasaPowerService.GetAll());
 
    /// <summary>Obtém um registro NASA POWER pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(NasaPowerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var n = nasaPowerService.GetById(id);
        return n is null ? NotFound() : Ok(n);
    }
 
    /// <summary>Lista todos os registros NASA POWER de um talhão específico.</summary>
    [HttpGet("by-talhao/{talhaoId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<NasaPowerResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByTalhao(Guid talhaoId) =>
        Ok(nasaPowerService.GetByTalhaoId(talhaoId));
 
    /// <summary>Cadastra um novo registro NASA POWER.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(NasaPowerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] NasaPowerRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = nasaPowerService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
 
    /// <summary>Remove um registro NASA POWER pelo ID.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        nasaPowerService.Delete(id) ? NoContent() : NotFound();
}