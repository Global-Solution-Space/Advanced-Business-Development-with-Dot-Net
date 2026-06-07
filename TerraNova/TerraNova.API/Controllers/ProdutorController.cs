using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.API.Controllers;

/// <summary>Gerenciamento de produtores rurais.</summary>
[Tags("Produtor")]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ProdutorController(IProdutorService produtorService) : ControllerBase
{
    /// <summary>Lista todos os produtores</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProdutorResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(produtorService.GetAll());
 
    /// <summary>Obtém um produtor pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProdutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var p = produtorService.GetById(id);
        return p is null ? NotFound() : Ok(p);
    }
 
    /// <summary>Obtém um produtor pelo e-mail.</summary>
    [HttpGet("by-email")]
    [ProducesResponseType(typeof(ProdutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetByEmail([FromQuery] string email)
    {
        var p = produtorService.GetByEmail(email);
        return p is null ? NotFound() : Ok(p);
    }
 
    /// <summary>Cadastra um novo produtor.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] ProdutorRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = produtorService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
 
    /// <summary>Atualiza um produtor pelo ID.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProdutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, [FromBody] ProdutorRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            return Ok(produtorService.Update(id, request));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    /// <summary>Remove um produtor pelo ID.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        produtorService.Delete(id) ? NoContent() : NotFound();
}