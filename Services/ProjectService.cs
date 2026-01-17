using Microsoft.EntityFrameworkCore;
using TaskMgmt.Context;
using TaskMgmt.Interfaces;
using TaskMgmt.Models;
using Microsoft.Extensions.Logging;

namespace TaskMgmt.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(AppDbContext db, ILogger<ProjectService> logger)
    {
        _db = db;
         _logger = logger;
    }

    public async Task<List<Project>> GetAllProjectsAsync()
    {
        _logger.LogInformation("Fetching all projects");
        return await _db.Projects
            .Include(p => p.Tasks)
            .ToListAsync();
    }

    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        _logger.LogInformation("Fetching project with ID {ProjectId}", id);
        return await _db.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Project> CreateProjectAsync(Project project)
    {
        _logger.LogInformation( "Creating project with name {ProjectName} for UserId {UserId}", project.Name, project.UserId);
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Project created successfully with ID {ProjectId}", project.Id);

        return project;
    }

    public async Task<Project?> UpdateProjectAsync(Project project)
    {
        _logger.LogInformation("Updating project with ID {ProjectId}", project.Id);
        var existing = await _db.Projects.FindAsync(project.Id);
        if (existing == null)
        {
            _logger.LogWarning("Update failed. Project with ID {ProjectId} not found", project.Id );  
            return null;
        }

        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Project with ID {ProjectId} updated successfully", project.Id );
        return existing;
    }

    public async Task<bool> DeleteProjectAsync(int id)
    {
         _logger.LogInformation("Deleting project with ID {ProjectId}", id);
        var proj = await _db.Projects.FindAsync(id);
       if (proj == null)
        {
            _logger.LogWarning("Delete failed. Project with ID {ProjectId} not found", id );
            return false;
        }

        proj.IsDeleted = true;
        proj.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Project with ID {ProjectId} marked as deleted", id);
        return true;
    }
}
