using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaskMgmt.Interfaces;
using TaskMgmt.DTOs;
using TaskMgmt.Models;
using TaskMgmt.Common;

namespace TaskMgmt.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectController(IProjectService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var projects = await _service.GetAllProjectsAsync();

        var response = projects.Select(p => new ProjectResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            UserId = p.UserId,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();

        return Ok(new ApiResponse<List<ProjectResponse>>
        {
            Success = true,
            Message = "Projects fetched successfully",
            Data = response
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var project = await _service.GetProjectByIdAsync(id);
        if (project == null)
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Project not found"
            });

        var response = new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            UserId = project.UserId,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };

        return Ok(new ApiResponse<ProjectResponse>
        {
            Success = true,
            Message = "Project fetched successfully",
            Data = response
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProjectCreate dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            UserId = dto.UserId
        };

        var created = await _service.CreateProjectAsync(project);

        return Ok(new ApiResponse<ProjectResponse>
        {
            Success = true,
            Message = "Project created",
            Data = new ProjectResponse
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                UserId = created.UserId,
                CreatedAt = created.CreatedAt,
                UpdatedAt = created.UpdatedAt
            }
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProjectUpdate dto)
    {
        var existing = await _service.GetProjectByIdAsync(id);
        if (existing == null)
            return NotFound();

        existing.Name = dto.Name;
        existing.Description = dto.Description;

        var updated = await _service.UpdateProjectAsync(existing);

        return Ok(new ApiResponse<ProjectResponse>
        {
            Success = true,
            Message = "Project updated",
            Data = new ProjectResponse
            {
                Id = updated!.Id,
                Name = updated.Name,
                Description = updated.Description,
                UserId = updated.UserId,
                CreatedAt = updated.CreatedAt,
                UpdatedAt = updated.UpdatedAt
            }
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteProjectAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
