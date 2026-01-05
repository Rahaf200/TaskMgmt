using Microsoft.EntityFrameworkCore;
using TaskMgmt.Context;
using TaskMgmt.Interfaces;
using TaskMgmt.Models;
using Microsoft.Extensions.Logging;

namespace TaskMgmt.Services;

public class TaskItemService : ITaskItemService
{
    private readonly AppDbContext _db;
    private readonly ILogger<TaskItemService> _logger;

    public TaskItemService(AppDbContext db, ILogger<TaskItemService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        _logger.LogInformation("Fetching all tasks");
        return await _db.TaskItems
            .Include(t => t.Comments)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        _logger.LogInformation("Fetching task with ID {TaskId}", id);
        return await _db.TaskItems
            .Include(t => t.Comments)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<IEnumerable<TaskItem>> GetTasksByProjectIdAsync(int projectId)
    {
        _logger.LogInformation("Fetching tasks for ProjectId {ProjectId}", projectId);
        return await _db.TaskItems
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.Comments)
            .ToListAsync();
    }
    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        _logger.LogInformation("Creating task '{Title}' for ProjectId {ProjectId} by UserId {UserId}", task.Title, task.ProjectId, task.UserId);
        task.CreatedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        _db.TaskItems.Add(task);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Task created successfully with ID {TaskId}", task.Id);  
        return task;
    }

    public async Task<TaskItem?> UpdateTaskAsync(TaskItem task)
    {
        _logger.LogInformation("Updating task with ID {TaskId}", task.Id);
        var existing = await _db.TaskItems.FindAsync(task.Id);
         if (existing == null)
        {
            _logger.LogWarning("Update failed. Task with ID {TaskId} not found", task.Id);
            return null;
        }

        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.Status = task.Status;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Task with ID {TaskId} updated successfully", task.Id);  
        return existing;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        _logger.LogInformation("Deleting task with ID {TaskId}", id);
        var t = await _db.TaskItems.FindAsync(id);
        if (t == null)
        {
            _logger.LogWarning("Delete failed. Task with ID {TaskId} not found", id);
            return false;
        }

        t.IsDeleted = true;
        t.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

         _logger.LogInformation("Task with ID {TaskId} marked as deleted", id);

        return true;
    }
}
