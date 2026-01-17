using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskMgmt.Interfaces;
using TaskMgmt.DTOs;
using TaskMgmt.Models;
using TaskMgmt.Common;

namespace TaskMgmt.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks/{taskId}/comments")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _service;

    public CommentController(ICommentService service)
    {
        _service = service;
    }

    [HttpGet]
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
    public async Task<IActionResult> Create(int taskId, CommentCreate dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var comment = new Comment
        {
            Content = dto.Content,
            TaskItemId = taskId,
            CreatedByUserId = userId
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
    public async Task<IActionResult> Delete(int taskId, int id)
    {
        var comment = await _service.GetCommentByIdAsync(id);
        if (comment == null || comment.TaskItemId != taskId)
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Comment not found",
                Data = null
            });

        await _service.DeleteCommentAsync(id);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Comment deleted successfully",
            Data = null
        });
    }
}
