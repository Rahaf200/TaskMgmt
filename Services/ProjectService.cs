using Microsoft.EntityFrameworkCore;
using TaskMgmt.Context;
using TaskMgmt.Interfaces;
using TaskMgmt.Models;

namespace TaskMgmt.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _db;

    public ProjectService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Project>> GetAllProjectsAsync()
    {
        return await _db.Projects
            .Include(p => p.Tasks)
            .ToListAsync();
    }

    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        return await _db.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Project> CreateProjectAsync(Project project)
    {
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();
        return project;
    }

    public async Task<Project?> UpdateProjectAsync(Project project)
    {
        var existing = await _db.Projects.FindAsync(project.Id);
        if (existing == null) return null;

        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteProjectAsync(int id)
    {
        var proj = await _db.Projects.FindAsync(id);
        if (proj == null) return false;

        _db.Projects.Remove(proj);
        await _db.SaveChangesAsync();
        return true;
    }
}
