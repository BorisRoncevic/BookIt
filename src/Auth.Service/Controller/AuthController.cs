using Microsoft.AspNetCore.Mvc;
using Auth.Service.Models;
using Auth.Service.Services;

namespace Auth.Service.Controller;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var token = await _service.RegisterAsync(request);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var token = await _service.LoginAsync(request);
            return Ok(new { token });
        }
        catch
        {
            return Unauthorized("Invalid credentials");
        }
    }
}
