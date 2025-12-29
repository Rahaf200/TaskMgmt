using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskMgmt.Interfaces;
using TaskMgmt.DTOs;
using TaskMgmt.Models;
using TaskMgmt.Common;

namespace TaskMgmt.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    // 🔹 UPDATED constructor
    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // GET api/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Fetching all users");

        var users = await _userService.GetAllUsersAsync();

        var response = users.Select(u => new UserResponse
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        }).ToList();

        return Ok(new ApiResponse<List<UserResponse>>
        {
            Success = true,
            Message = "Users fetched successfully",
            Data = response
        });
    }

    // GET api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        _logger.LogInformation("Fetching user with id {UserId}", id);

        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "User not found",
                Data = null
            });

        var response = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return Ok(new ApiResponse<UserResponse>
        {
            Success = true,
            Message = "User fetched successfully",
            Data = response
        });
    }

    // POST api/users
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserCreate dto)
    {
        _logger.LogInformation("Creating new user with username {Username}", dto.Username);

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        var created = await _userService.CreateUserAsync(user);

        var response = new UserResponse
        {
            Id = created.Id,
            Username = created.Username,
            Email = created.Email,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };

        return CreatedAtAction(nameof(Get), new { id = response.Id },
            new ApiResponse<UserResponse>
            {
                Success = true,
                Message = "User created successfully",
                Data = response
            });
    }

    // PUT api/users/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdate dto)
    {
        _logger.LogInformation("Updating user with id {UserId}", id);

        var existing = await _userService.GetUserByIdAsync(id);
        if (existing == null)
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "User not found"
            });

        existing.Username = dto.Username;
        existing.Email = dto.Email;

        var updated = await _userService.UpdateUserAsync(existing);

        var response = new UserResponse
        {
            Id = updated!.Id,
            Username = updated.Username,
            Email = updated.Email,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        };

        return Ok(new ApiResponse<UserResponse>
        {
            Success = true,
            Message = "User updated successfully",
            Data = response
        });
    }

    // DELETE api/users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting user with id {UserId}", id);

        var success = await _userService.DeleteUserAsync(id);

        if (!success)
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "User not found",
                Data = null
            });

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "User deleted successfully",
            Data = null
        });
    }
}
