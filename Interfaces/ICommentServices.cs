using TaskMgmt.Models;
using TaskMgmt.DTOs;

namespace TaskMgmt.Interfaces;

public interface ICommentService
{
    Task<List<Comment>> GetCommentsByTaskIdAsync(int taskId);
     Task<Comment?> GetCommentByIdAsync(int id);
    Task<Comment> CreateCommentAsync(Comment comment);
    Task<Comment?> UpdateCommentAsync(Comment comment);
    Task<bool> DeleteCommentAsync(int id);

}