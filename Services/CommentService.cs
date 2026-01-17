using Microsoft.EntityFrameworkCore;
using TaskMgmt.Context;
using TaskMgmt.Interfaces;
using TaskMgmt.Models;
using Microsoft.Extensions.Logging;

namespace TaskMgmt.Services;

public class CommentService : ICommentService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CommentService> _logger;

    public CommentService(AppDbContext context, ILogger<CommentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Comment>> GetCommentsByTaskIdAsync(int taskId)
    {
        _logger.LogInformation("Fetching comments for TaskId {TaskId}", taskId);
        return await _context.Comments
            .Where(c => c.TaskItemId == taskId)
            .ToListAsync();
    }

    public async Task<Comment?> GetCommentByIdAsync(int id)
    {
        return await _context.Comments.FindAsync(id);
    }

    public async Task<Comment> CreateCommentAsync(Comment comment)
    {
        comment.CreatedAt = DateTime.UtcNow;
        comment.UpdatedAt = DateTime.UtcNow;

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Comment created successfully. CommentId: {CommentId}", comment.Id);

        return comment;
    }

     public async Task<Comment?> UpdateCommentAsync(Comment comment)
    {
        var existing = await _context.Comments.FindAsync(comment.Id);
        if (existing == null)
        {
            _logger.LogWarning("Update failed. Comment not found. CommentId: {CommentId}", comment.Id);

            return null;
        }

        existing.Content = comment.Content;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Comment updated successfully. CommentId: {CommentId}", existing.Id);

        return existing;
    }

    public async Task<bool> DeleteCommentAsync(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            _logger.LogWarning("Delete failed. Comment not found. Id {Id}", id); // 🔹 ADDED
            return false;
        }
           
        comment.IsDeleted = true;
        comment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
         _logger.LogInformation("Comment deleted successfully. CommentId: {CommentId}", id);
        return true;
    }
}
