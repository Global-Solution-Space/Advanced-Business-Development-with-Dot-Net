using Microsoft.AspNetCore.Mvc;
using TerraNova.Application.DTOs;
using TerraNova.Application.Services.Interfaces;
using TerraNova.Domain.Enums;

namespace TerraNova.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ReqApiController(IReqApiService reqApiService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ReqApiResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(reqApiService.GetAll());

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReqApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var r = reqApiService.GetById(id);
        return r is null ? NotFound() : Ok(r);
    }

    [HttpGet("talhao/{talhaoId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<ReqApiResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByTalhaoId(Guid talhaoId) =>
        Ok(reqApiService.GetByTalhaoId(talhaoId));


    [HttpPost]
    [ProducesResponseType(typeof(ReqApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ReqApiRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await reqApiService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id) =>
        reqApiService.Delete(id) ? NoContent() : NotFound();
}
