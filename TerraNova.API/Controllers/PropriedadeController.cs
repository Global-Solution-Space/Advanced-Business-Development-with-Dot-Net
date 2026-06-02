using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.API.Controllers;

/// <summary>Propriedades rurais. Pertencem a um Produtor e possuem Localização exclusiva.</summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class PropriedadeController(IPropriedadeService propriedadeService) : ControllerBase
{
    /// <summary>Lista todas as propriedades cadastradas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PropriedadeResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(propriedadeService.GetAll());
 
    /// <summary>Obtém uma propriedade pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PropriedadeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var p = propriedadeService.GetById(id);
        return p is null ? NotFound() : Ok(p);
    }
 
    /// <summary>Lista todas as propriedades de um produtor específico.</summary>
    [HttpGet("by-produtor/{produtorId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<PropriedadeResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByProdutor(Guid produtorId) =>
        Ok(propriedadeService.GetByProdutorId(produtorId));
 
    /// <summary>Cadastra uma nova propriedade.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PropriedadeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] PropriedadeRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = propriedadeService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
 
    /// <summary>Remove uma propriedade pelo ID.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        propriedadeService.Delete(id) ? NoContent() : NotFound();
}