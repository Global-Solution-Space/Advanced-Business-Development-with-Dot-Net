using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.API.Controllers;

/// <summary>Gerenciamento dos tipos de APIs externas. NASAPOWER/SATVEG)</summary>
[Tags("Tipo de API")]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class TipoApiController(ITipoApiService tipoApiService) : ControllerBase
{
    /// <summary>Lista todos os tipos de API.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TipoApiResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(tipoApiService.GetAll());

    /// <summary>Obtém um tipo de API pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TipoApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var t = tipoApiService.GetById(id);
        return t is null ? NotFound() : Ok(t);
    }

    /// <summary>Cria um novo tipo de API.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TipoApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] TipoApiRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = tipoApiService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Atualiza um tipo de API pelo ID.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TipoApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, [FromBody] TipoApiRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            return Ok(tipoApiService.Update(id, request));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    /// <summary>Remove um tipo de API pelo ID.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        tipoApiService.Delete(id) ? NoContent() : NotFound();
}