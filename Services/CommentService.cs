using Microsoft.EntityFrameworkCore;
using TaskMgmt.Context;
using TaskMgmt.Interfaces;
using TaskMgmt.Models;

namespace TaskMgmt.Services;

public class CommentService : ICommentService
{
    private readonly AppDbContext _context;

    public CommentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Comment>> GetCommentsByTaskIdAsync(int taskId)
    {
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

        return comment;
    }

    public async Task<Comment?> UpdateCommentAsync(Comment comment)
    {
        comment.UpdatedAt = DateTime.UtcNow;

        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();

        return comment;
    }

    public async Task<bool> DeleteCommentAsync(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
            return false;

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return true;
    }
}
