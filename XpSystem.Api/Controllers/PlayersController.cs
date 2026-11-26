using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XpSystem;
using XpSystem.Api.Dtos;

namespace XpSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly PlayerService _service;

    public PlayersController(PlayerService service) => _service = service;

    [HttpPost]
    public IActionResult Create([FromBody] CreatePlayerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("name is required");

        return Ok(_service.CreatePlayer(request.Name));
    }

    [HttpGet]
    public IActionResult GetMine() => Ok(_service.GetMyPlayers());

    [HttpGet("{id:int}")]
    public IActionResult GetOne(int id)
        => _service.GetPlayer(id) is { } player ? Ok(player) : NotFound();

    [HttpPost("{id:int}/xp")]
    public IActionResult GainXp(int id, [FromBody] GainXpRequest request)
    {
        try
        {
            _service.GainXp(id, request.Amount);
            return NoContent();
        }
        catch (ArgumentOutOfRangeException) { return BadRequest("amount must be >= 0"); }
        catch (InvalidOperationException) { return NotFound(); }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            _service.DeletePlayer(id);
            return NoContent();
        }
        catch (InvalidOperationException) { return NotFound(); }
    }
}
