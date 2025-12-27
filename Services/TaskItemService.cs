using Microsoft.EntityFrameworkCore;
using TaskMgmt.Context;
using TaskMgmt.Interfaces;
using TaskMgmt.Models;

namespace TaskMgmt.Services;

public class TaskItemService : ITaskItemService
{
    private readonly AppDbContext _db;

    public TaskItemService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        return await _db.TaskItems
            .Include(t => t.Comments)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        return await _db.TaskItems
            .Include(t => t.Comments)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<IEnumerable<TaskItem>> GetTasksByProjectIdAsync(int projectId)
    {
        return await _db.TaskItems
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.Comments)
            .ToListAsync();
    }
    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        _db.TaskItems.Add(task);
        await _db.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateTaskAsync(TaskItem task)
    {
        var existing = await _db.TaskItems.FindAsync(task.Id);
        if (existing == null) return null;

        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.Status = task.Status;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var t = await _db.TaskItems.FindAsync(id);
        if (t == null) return false;

        _db.TaskItems.Remove(t);
        await _db.SaveChangesAsync();
        return true;
    }
}
