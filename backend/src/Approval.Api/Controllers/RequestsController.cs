using System.Security.Claims;
using Approval.Application.Contracts;
using Approval.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Approval.Api.Controllers;
[ApiController, Route("api/requests"), Authorize]
public sealed class RequestsController(IRequestService requestService, IIdentityService identityService) : ControllerBase
{
    [HttpPost, Authorize(Roles = "Employee")]
    public async Task<IActionResult> Create(CreateRequestDto dto, CancellationToken ct) => Ok(await requestService.CreateAsync(GetUserId(), dto, ct));
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    { var userId = GetUserId(); var roles = await identityService.GetRolesAsync(userId, ct); return Ok(await requestService.GetVisibleAsync(userId, roles, ct)); }
    [HttpPost("{id:guid}/decision")]
    public async Task<IActionResult> Decide(Guid id, DecisionDto dto, CancellationToken ct)
    { var userId = GetUserId(); var roles = await identityService.GetRolesAsync(userId, ct); return Ok(await requestService.DecideAsync(id, userId, roles, dto.Action, ct)); }
    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Authenticated user id is missing.");
}
