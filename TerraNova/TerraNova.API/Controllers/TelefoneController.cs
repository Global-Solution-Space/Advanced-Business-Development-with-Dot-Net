using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.API.Controllers;

/// <summary>Gerenciamento de contatos telefônicos.</summary>
[Tags("Telefone")]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class TelefoneController(ITelefoneService telefoneService) : ControllerBase
{
    /// <summary>Lista todos os registros de telefone cadastrados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TelefoneResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(telefoneService.GetAll());
 
    /// <summary>Obtém um telefone pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TelefoneResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var t = telefoneService.GetById(id);
        return t is null ? NotFound() : Ok(t);
    }
 
    /// <summary>Obtém o telefone detalhado de um produtor específico.</summary>
    [HttpGet("by-produtor/{produtorId:guid}")]
    [ProducesResponseType(typeof(TelefoneResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetByProdutor(Guid produtorId)
    {
        var t = telefoneService.GetByProdutorId(produtorId);
        return t is null ? NotFound() : Ok(t);
    }
 
    /// <summary>Cadastra o telefone detalhado de um produtor.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TelefoneResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] TelefoneRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = telefoneService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
 
    /// <summary>Atualiza o telefone detalhado de um produtor.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TelefoneResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, [FromBody] TelefoneUpdateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            return Ok(telefoneService.Update(id, request));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    /// <summary>Remove um telefone pelo ID.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        telefoneService.Delete(id) ? NoContent() : NotFound();
}