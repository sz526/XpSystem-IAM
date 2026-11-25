using Microsoft.AspNetCore.Mvc;
using XpSystem;
using XpSystem.Api.Dtos;
using Microsoft.AspNetCore.Authorization;


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

        var player = _service.CreatePlayer(request.Name);
        return Ok(player);
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_service.GetAllPlayers());

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
}
