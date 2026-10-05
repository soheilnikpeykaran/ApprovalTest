using Approval.Application.Contracts;
using Approval.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace Approval.Api.Controllers;
[ApiController][Route("api/auth")]
public sealed class AuthController(IIdentityService identityService) : ControllerBase
{
 [HttpPost("register")] public async Task<IActionResult> Register(RegisterRequest request,CancellationToken ct){var r=await identityService.RegisterAsync(request,ct); return r is null?Conflict(new{message="Email is already registered."}):Ok(r);}
 [HttpPost("login")] public async Task<IActionResult> Login(LoginRequest request,CancellationToken ct){var r=await identityService.LoginAsync(request,ct); return r is null?Unauthorized(new{message="Invalid credentials."}):Ok(r);}
}
