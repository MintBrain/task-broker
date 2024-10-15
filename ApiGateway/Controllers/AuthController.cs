using Microsoft.AspNetCore.Mvc;
using ApiGateway.Services;
using ApiGateway.Models;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AuthRequest request)
    {
        // Здесь можно реализовать логику проверки имени пользователя и пароля
        if (request.Username == "test" && request.Password == "password") // Простая проверка
        {
            var token = _authService.GenerateJwtToken(request.Username);
            return Ok(new { token });
        }

        return Unauthorized();
    }
}