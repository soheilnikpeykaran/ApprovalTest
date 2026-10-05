using Approval.Application.Contracts;
using Approval.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace Approval.Api.Controllers;
[ApiController, Route("api/auth")]
public sealed class AuthController(IIdentityService identityService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    { var result = await identityService.RegisterAsync(request, ct); return result is null ? BadRequest(new { message = "Registration failed. Check the supplied data." }) : Ok(result); }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    { var result = await identityService.LoginAsync(request, ct); return result is null ? Unauthorized(new { message = "Invalid email or password." }) : Ok(result); }
}
