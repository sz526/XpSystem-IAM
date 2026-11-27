using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XpSystem;

namespace XpSystem.Api.Controllers;

[Authorize(Policy = "Admin")]
[ApiController]
[Route("api/admin/players")]
public class AdminPlayersController : ControllerBase
{
    private readonly PlayerService _service;

    public AdminPlayersController(PlayerService service) => _service = service;

    [HttpGet]
    public IActionResult GetAll() => Ok(_service.GetAllPlayers());
}
