using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace TerraNova.API.Controllers;

/// <summary>Alertas baseados na análise cruzada dos dados.</summary>
[Tags("Alerta Agrícola")]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AlertaAgricolaController(IAlertaAgricolaService alertaService) : ControllerBase
{
    /// <summary>Lista todos os alertas agrícolas</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AlertaAgricolaResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(alertaService.GetAll());

    /// <summary>Obtém um alerta pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AlertaAgricolaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var a = alertaService.GetById(id);
        return a is null ? NotFound() : Ok(a);
    }

    /// <summary>Lista todos os alertas de um talhão específico.</summary>
    [HttpGet("talhao/{talhaoId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<AlertaAgricolaResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByTalhaoId(Guid talhaoId) =>
        Ok(alertaService.GetByTalhaoId(talhaoId));

    /// <summary>Cria um novo alerta agrícola manual.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AlertaAgricolaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] AlertaAgricolaRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = alertaService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Atualiza um alerta agrícola pelo ID.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AlertaAgricolaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, [FromBody] AlertaAgricolaRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            return Ok(alertaService.Update(id, request));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    /// <summary>Resolver Alerta</summary>
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

    /// <summary>Reabrir Alerta</summary>
    [HttpPatch("{id:guid}/reabrir")]
    [ProducesResponseType(typeof(AlertaAgricolaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Reabrir(Guid id)
    {
        try
        {
            return Ok(alertaService.Reabrir(id));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    /// <summary>Remove um alerta pelo ID.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        alertaService.Delete(id) ? NoContent() : NotFound();
}