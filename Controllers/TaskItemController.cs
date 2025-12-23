using Microsoft.AspNetCore.Mvc;
using TaskMgmt.Interfaces;
using TaskMgmt.DTOs;
using TaskMgmt.Models;
using TaskMgmt.Common;

namespace TaskMgmt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskItemController : ControllerBase
{
    private readonly ITaskItemService _service;

    public TaskItemController(ITaskItemService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _service.GetAllTasksAsync();

        var response = tasks.Select(t => new TaskResponse
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            UserId = t.UserId,
            ProjectId = t.ProjectId,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList();

        return Ok(new ApiResponse<List<TaskResponse>>
        {
            Success = true,
            Message = "Tasks fetched",
            Data = response
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskCreate dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            UserId = dto.UserId,
            ProjectId = dto.ProjectId
        };

        var created = await _service.CreateTaskAsync(task);

        return Ok(new ApiResponse<TaskResponse>
        {
            Success = true,
            Message = "Task created",
            Data = new TaskResponse
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description,
                Status = created.Status,
                UserId = created.UserId,
                ProjectId = created.ProjectId,
                CreatedAt = created.CreatedAt,
                UpdatedAt = created.UpdatedAt
            }
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TaskUpdate dto)
    {
        var existing = await _service.GetTaskByIdAsync(id);
        if (existing == null)
            return NotFound();

        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.Status = dto.Status;

        var updated = await _service.UpdateTaskAsync(existing);

        return Ok(new ApiResponse<TaskResponse>
        {
            Success = true,
            Message = "Task updated",
            Data = new TaskResponse
            {
                Id = updated!.Id,
                Title = updated.Title,
                Description = updated.Description,
                Status = updated.Status,
                UserId = updated.UserId,
                ProjectId = updated.ProjectId,
                CreatedAt = updated.CreatedAt,
                UpdatedAt = updated.UpdatedAt
            }
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteTaskAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
