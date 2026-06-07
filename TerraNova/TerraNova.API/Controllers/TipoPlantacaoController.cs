using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.API.Controllers;

/// <summary>Gerenciamento dos tipos de culturas.</summary>
[Tags("Tipo de Plantação")]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class TipoPlantacaoController(ITipoPlantacaoService tipoPlantacaoService) : ControllerBase
{
    /// <summary>Lista todos os tipos de plantação cadastrados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TipoPlantacaoResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(tipoPlantacaoService.GetAll());
 
    /// <summary>Obtém um tipo de plantação pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TipoPlantacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var t = tipoPlantacaoService.GetById(id);
        return t is null ? NotFound() : Ok(t);
    }
 
    /// <summary>Cadastra um novo tipo de plantação.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TipoPlantacaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] TipoPlantacaoRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = tipoPlantacaoService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
 
    /// <summary>Atualiza um tipo de plantação pelo ID.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TipoPlantacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, [FromBody] TipoPlantacaoRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            return Ok(tipoPlantacaoService.Update(id, request));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    /// <summary>Remove um tipo de plantação pelo ID.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        tipoPlantacaoService.Delete(id) ? NoContent() : NotFound();
}