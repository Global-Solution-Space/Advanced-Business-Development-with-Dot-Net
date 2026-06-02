using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.API.Controllers;

/// <summary>Análises de vegetação SatVeg. Vinculadas a um Talhão.</summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class SatvegController(ISatvegService satvegService) : ControllerBase
{
    /// <summary>Lista todas as análises SatVeg cadastradas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SatvegResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(satvegService.GetAll());
 
    /// <summary>Obtém uma análise SatVeg pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SatvegResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var s = satvegService.GetById(id);
        return s is null ? NotFound() : Ok(s);
    }
 
    /// <summary>Lista todas as análises SatVeg de um talhão específico.</summary>
    [HttpGet("by-talhao/{talhaoId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<SatvegResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByTalhao(Guid talhaoId) =>
        Ok(satvegService.GetByTalhaoId(talhaoId));
 
    /// <summary>Cadastra uma nova análise SatVeg.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(SatvegResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] SatvegRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = satvegService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
 
    /// <summary>Remove uma análise SatVeg pelo ID.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        satvegService.Delete(id) ? NoContent() : NotFound();
}