using TaskMgmt.Models;
namespace TaskMgmt.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(User user);
    Task<User?> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
}