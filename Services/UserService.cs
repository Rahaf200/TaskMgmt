using Microsoft.EntityFrameworkCore;
using TaskMgmt.Context;
using TaskMgmt.Interfaces;
using TaskMgmt.Models;
using Microsoft.Extensions.Logging;

namespace TaskMgmt.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext db, ILogger<UserService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        _logger.LogInformation("Fetching all users");
        return await _db.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        _logger.LogInformation("Fetching user with ID {UserId}", id);
        return await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

     public async Task<User?> GetByUsernameAsync(string username)
    {
        _logger.LogInformation("Fetching user with username {Username}", username);
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        _logger.LogInformation("Creating user with Username {Username} and Email {Email}", user.Username, user.Email);
        var exists = await _db.Users.AnyAsync(u =>
            u.Username == user.Username || u.Email == user.Email);

        if (exists)
        {
            _logger.LogWarning("User creation failed. Username or Email already exists. Username: {Username}, Email: {Email}", user.Username, user.Email);   
            throw new InvalidOperationException("Username or email already exists");
        }
            
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        _logger.LogInformation("User created successfully with ID {UserId}", user.Id);
        return user;
    }

    public async Task<User?> UpdateUserAsync(User user)
    {
        _logger.LogInformation("Updating user with ID {UserId}", user.Id);
        var existing = await _db.Users.FindAsync(user.Id);
        if (existing == null)
        {
            _logger.LogWarning("Update failed. User with ID {UserId} not found", user.Id);
            return null;
        }

        existing.Username = user.Username;
        existing.Email = user.Email;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        _logger.LogInformation("User with ID {UserId} updated successfully", user.Id);
        return existing;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        _logger.LogInformation("Deleting user with ID {UserId}", id);
        var u = await _db.Users.FindAsync(id);
        if (u == null)
        {
            _logger.LogWarning("Delete failed. User with ID {UserId} not found", id);
            return false;
        }

        u.IsDeleted = true; 
        u.UpdatedAt = DateTime.UtcNow;
        
        await _db.SaveChangesAsync();
        _logger.LogInformation("User with ID {UserId} marked as deleted", id);
        return true;
    }
}
