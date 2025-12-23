using Microsoft.AspNetCore.Mvc;
using TaskMgmt.Interfaces;
using TaskMgmt.DTOs;
using TaskMgmt.Models;
using TaskMgmt.Common;

namespace TaskMgmt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _service;

    public CommentController(ICommentService service)
    {
        _service = service;
    }

    [HttpGet("task/{taskId}")]
    public async Task<IActionResult> GetByTask(int taskId)
    {
        var comments = await _service.GetCommentsByTaskIdAsync(taskId);

        var response = comments.Select(c => new CommentResponse
        {
            Id = c.Id,
            Content = c.Content,
            TaskItemId = c.TaskItemId,
            UserId = c.CreatedByUserId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        return Ok(new ApiResponse<List<CommentResponse>>
        {
            Success = true,
            Message = "Comments fetched",
            Data = response
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CommentCreate dto)
    {
        var comment = new Comment
        {
            Content = dto.Content,
            TaskItemId = dto.TaskItemId,
            CreatedByUserId = dto.UserId
        };

        var created = await _service.CreateCommentAsync(comment);

        return Ok(new ApiResponse<CommentResponse>
        {
            Success = true,
            Message = "Comment created",
            Data = new CommentResponse
            {
                Id = created.Id,
                Content = created.Content,
                TaskItemId = created.TaskItemId,
                UserId = created.CreatedByUserId,
                CreatedAt = created.CreatedAt,
                UpdatedAt = created.UpdatedAt
            }
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteCommentAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
