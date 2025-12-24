using Microsoft.EntityFrameworkCore;
using TaskMgmt.Context;
using TaskMgmt.Interfaces;
using TaskMgmt.Models;

namespace TaskMgmt.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

     public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateUserAsync(User user)
    {
        var existing = await _db.Users.FindAsync(user.Id);
        if (existing == null) return null;

        existing.Username = user.Username;
        existing.Email = user.Email;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var u = await _db.Users.FindAsync(id);
        if (u == null) return false;

        _db.Users.Remove(u);
        await _db.SaveChangesAsync();
        return true;
    }
}
