using System.Security.Claims;
using ImmobilienVerwalter.API.DTOs;
using ImmobilienVerwalter.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImmobilienVerwalter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
        => _dashboardService = dashboardService;

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var dashboard = await _dashboardService.GetDashboardAsync(userId);
        return Ok(dashboard);
    }
}
