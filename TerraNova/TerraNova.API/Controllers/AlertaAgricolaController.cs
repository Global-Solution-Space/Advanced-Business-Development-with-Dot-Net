using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AlertaAgricolaController(IAlertaAgricolaService alertaService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AlertaAgricolaResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(alertaService.GetAll());

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AlertaAgricolaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var a = alertaService.GetById(id);
        return a is null ? NotFound() : Ok(a);
    }

    [HttpGet("talhao/{talhaoId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<AlertaAgricolaResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByTalhaoId(Guid talhaoId) =>
        Ok(alertaService.GetByTalhaoId(talhaoId));

    [HttpPost]
    [ProducesResponseType(typeof(AlertaAgricolaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] AlertaAgricolaRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = alertaService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/resolver")]
    [ProducesResponseType(typeof(AlertaAgricolaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Resolver(Guid id)
    {
        try
        {
            return Ok(alertaService.Resolver(id));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        alertaService.Delete(id) ? NoContent() : NotFound();
}
