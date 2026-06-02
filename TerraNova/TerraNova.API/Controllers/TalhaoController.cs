using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.API.Controllers;

/// <summary>Talhões. Subdivisão de uma Propriedade; origem dos dados SatVeg e NASA POWER.</summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class TalhaoController(ITalhaoService talhaoService) : ControllerBase
{
    /// <summary>Lista todos os talhões cadastrados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TalhaoResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(talhaoService.GetAll());
 
    /// <summary>Obtém um talhão pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TalhaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var t = talhaoService.GetById(id);
        return t is null ? NotFound() : Ok(t);
    }
 
    /// <summary>Lista todos os talhões de uma propriedade específica.</summary>
    [HttpGet("by-propriedade/{propriedadeId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<TalhaoResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByPropriedade(Guid propriedadeId) =>
        Ok(talhaoService.GetByPropriedadeId(propriedadeId));
 
    /// <summary>Lista todos os talhões de um tipo de plantação específico.</summary>
    [HttpGet("by-tipo-plantacao/{tipoPlantacaoId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<TalhaoResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByTipoPlantacao(Guid tipoPlantacaoId) =>
        Ok(talhaoService.GetByTipoPlantacaoId(tipoPlantacaoId));
 
    /// <summary>Cadastra um novo talhão.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TalhaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] TalhaoRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = talhaoService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
 
    /// <summary>Remove um talhão pelo ID.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        talhaoService.Delete(id) ? NoContent() : NotFound();
}