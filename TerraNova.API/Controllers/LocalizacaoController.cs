using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;

namespace Advanced_Business_Development_with_Dot_Net.Controllers;

/// <summary>Coordenadas geográficas. Pré-requisito para Propriedade e Talhão.</summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class LocalizacaoController(ILocalizacaoService localizacaoService) : ControllerBase
{
    /// <summary>Lista todas as localizações cadastradas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LocalizacaoResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(localizacaoService.GetAll());
 
    /// <summary>Obtém uma localização pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LocalizacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var loc = localizacaoService.GetById(id);
        return loc is null ? NotFound() : Ok(loc);
    }
 
    /// <summary>Cadastra uma nova localização.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(LocalizacaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] LocalizacaoRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = localizacaoService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
 
    /// <summary>Remove uma localização pelo ID.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        localizacaoService.Delete(id) ? NoContent() : NotFound();
}