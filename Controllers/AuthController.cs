using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskMgmt.Common;
using TaskMgmt.DTOs;
using TaskMgmt.Interfaces;

namespace TaskMgmt.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLogin dto)
    {
        var user = await _userService.GetByUsernameAsync(dto.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Invalid username or password",
                Data = null
            });
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, "User")
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("TASKMGMT_SUPER_SECRET_KEY_123")
        );

        var token = new JwtSecurityToken(
            issuer: "TaskMgmtAPI",
            audience: "TaskMgmtClient",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Login successful",
            Data = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
}
