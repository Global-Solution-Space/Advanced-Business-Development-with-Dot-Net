using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace Advanced_Business_Development_with_Dot_Net.Controllers;

/// <summary>Produtores rurais. Senha nunca é exposta nas respostas.</summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ProdutorController(IProdutorService produtorService) : ControllerBase
{
    /// <summary>Lista todos os produtores cadastrados.</summary>
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
 
    /// <summary>Remove um produtor pelo ID.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        produtorService.Delete(id) ? NoContent() : NotFound();
}