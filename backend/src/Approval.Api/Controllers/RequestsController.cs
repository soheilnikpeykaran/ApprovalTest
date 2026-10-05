using System.Security.Claims;
using Approval.Application.Contracts;
using Approval.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Approval.Api.Controllers;
[ApiController][Authorize][Route("api/requests")]
public sealed class RequestsController(IRequestService service) : ControllerBase
{
 [HttpPost][Authorize(Roles="Employee")] public async Task<IActionResult> Create(CreateRequestDto request,CancellationToken ct)=>Ok(await service.CreateAsync(UserId(),request,ct));
 [HttpGet] public async Task<IActionResult> Get(CancellationToken ct){var roles=User.FindAll(ClaimTypes.Role).Select(x=>x.Value).ToArray();return Ok(await service.GetVisibleAsync(UserId(),roles,ct));}
 [HttpPost("{id:guid}/decision")][Authorize(Roles="Manager,Finance")] public async Task<IActionResult> Decide(Guid id,[FromBody] DecisionDto dto,CancellationToken ct)=>Ok(await service.DecideAsync(id,UserId(),User.FindAll(ClaimTypes.Role).Select(x=>x.Value).ToArray(),dto.Action,ct));
 private string UserId()=>User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException("Invalid user identity.");
}
public sealed record DecisionDto(string Action);
