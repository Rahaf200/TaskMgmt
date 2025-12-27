using TaskMgmt.Models;

namespace TaskMgmt.Interfaces;

public interface ITaskItemService
{
    Task<List<TaskItem>> GetAllTasksAsync();
    Task<TaskItem?> GetTaskByIdAsync(int id);
    Task<TaskItem> CreateTaskAsync(TaskItem task);
    Task<TaskItem?> UpdateTaskAsync(TaskItem task);
    Task<bool> DeleteTaskAsync(int id);
     Task<IEnumerable<TaskItem>> GetTasksByProjectIdAsync(int projectId);
}