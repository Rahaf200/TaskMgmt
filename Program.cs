using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TaskMgmt.Context;
using TaskMgmt.Interfaces;
using TaskMgmt.Services;
using TaskMgmt.Middlewares;
using TaskMgmt.Common;
using TaskMgmt.Models;
using TaskMgmt.DTOs;

var builder = WebApplication.CreateBuilder(args);

// ===================== SERVICES =====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskItemService, TaskItemService>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===================== MIDDLEWARE =====================
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();


// ====================================================
// ===================== USERS =========================
// ====================================================

app.MapGet("/minimal/users", async (IUserService service) =>
{
    var users = await service.GetAllUsersAsync();

    return Results.Ok(new ApiResponse<List<UserResponse>>
    {
        Success = true,
        Message = "Users fetched",
        Data = users.Select(u => new UserResponse
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        }).ToList()
    });
});

app.MapGet("/minimal/users/{id:int}", async (int id, IUserService service) =>
{
    var user = await service.GetUserByIdAsync(id);
    if (user == null)
        return Results.NotFound(new ApiResponse<object> { Success = false, Message = "User not found" });

    return Results.Ok(new ApiResponse<UserResponse>
    {
        Success = true,
        Message = "User fetched",
        Data = new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        }
    });
});

app.MapPost("/minimal/users", async (UserCreate dto, IUserService service) =>
{
    var created = await service.CreateUserAsync(new User
    {
        Username = dto.Username,
        Email = dto.Email
    });

    return Results.Created($"/minimal/users/{created.Id}", new ApiResponse<UserResponse>
    {
        Success = true,
        Message = "User created",
        Data = new UserResponse
        {
            Id = created.Id,
            Username = created.Username,
            Email = created.Email,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        }
    });
});

app.MapPut("/minimal/users/{id:int}", async (int id, UserUpdate dto, IUserService service) =>
{
    var user = await service.GetUserByIdAsync(id);
    if (user == null)
        return Results.NotFound(new ApiResponse<object> { Success = false, Message = "User not found" });

    user.Username = dto.Username;
    user.Email = dto.Email;

    var updated = await service.UpdateUserAsync(user);

    return Results.Ok(new ApiResponse<UserResponse>
    {
        Success = true,
        Message = "User updated",
        Data = new UserResponse
        {
            Id = updated!.Id,
            Username = updated.Username,
            Email = updated.Email,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        }
    });
});

app.MapDelete("/minimal/users/{id:int}", async (int id, IUserService service) =>
{
    if (!await service.DeleteUserAsync(id))
        return Results.NotFound(new ApiResponse<object> { Success = false, Message = "User not found" });

    return Results.NoContent();
});


// ====================================================
// ==================== PROJECTS =======================
// ====================================================

app.MapGet("/minimal/projects", async (IProjectService service) =>
{
    var projects = await service.GetAllProjectsAsync();

    return Results.Ok(new ApiResponse<List<ProjectResponse>>
    {
        Success = true,
        Message = "Projects fetched",
        Data = projects.Select(p => new ProjectResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            UserId = p.UserId,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList()
    });
});

app.MapPost("/minimal/projects", async (ProjectCreate dto, IProjectService service) =>
{
    var created = await service.CreateProjectAsync(new Project
    {
        Name = dto.Name,
        Description = dto.Description,
        UserId = dto.UserId
    });

    return Results.Ok(new ApiResponse<ProjectResponse>
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
});

app.MapPut("/minimal/projects/{id:int}", async (int id, ProjectUpdate dto, IProjectService service) =>
{
    var project = await service.GetProjectByIdAsync(id);
    if (project == null)
        return Results.NotFound(new ApiResponse<object> { Success = false, Message = "Project not found" });

    project.Name = dto.Name;
    project.Description = dto.Description;

    var updated = await service.UpdateProjectAsync(project);

    return Results.Ok(new ApiResponse<ProjectResponse>
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
});

app.MapDelete("/minimal/projects/{id:int}", async (int id, IProjectService service) =>
{
    if (!await service.DeleteProjectAsync(id))
        return Results.NotFound(new ApiResponse<object> { Success = false, Message = "Project not found" });

    return Results.NoContent();
});


// ====================================================
// ====================== TASKS ========================
// ====================================================

app.MapGet("/minimal/tasks", async (ITaskItemService service) =>
{
    var tasks = await service.GetAllTasksAsync();

    return Results.Ok(new ApiResponse<List<TaskResponse>>
    {
        Success = true,
        Message = "Tasks fetched",
        Data = tasks.Select(t => new TaskResponse
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            UserId = t.UserId,
            ProjectId = t.ProjectId,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList()
    });
});

app.MapPost("/minimal/tasks", async (TaskCreate dto, ITaskItemService service) =>
{
    var created = await service.CreateTaskAsync(new TaskItem
    {
        Title = dto.Title,
        Description = dto.Description,
        Status = dto.Status,
        UserId = dto.UserId,
        ProjectId = dto.ProjectId
    });

    return Results.Ok(new ApiResponse<TaskResponse>
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
});

app.MapPut("/minimal/tasks/{id:int}", async (int id, TaskUpdate dto, ITaskItemService service) =>
{
    var task = await service.GetTaskByIdAsync(id);
    if (task == null)
        return Results.NotFound(new ApiResponse<object> { Success = false, Message = "Task not found" });

    task.Title = dto.Title;
    task.Description = dto.Description;
    task.Status = dto.Status;

    var updated = await service.UpdateTaskAsync(task);

    return Results.Ok(new ApiResponse<TaskResponse>
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
});

app.MapDelete("/minimal/tasks/{id:int}", async (int id, ITaskItemService service) =>
{
    if (!await service.DeleteTaskAsync(id))
        return Results.NotFound(new ApiResponse<object> { Success = false, Message = "Task not found" });

    return Results.NoContent();
});


// ====================================================
// ===================== COMMENTS ======================
// ====================================================

app.MapGet("/minimal/tasks/{taskId:int}/comments", async (int taskId, ICommentService service) =>
{
    var comments = await service.GetCommentsByTaskIdAsync(taskId);

    return Results.Ok(new ApiResponse<List<CommentResponse>>
    {
        Success = true,
        Message = "Comments fetched",
        Data = comments.Select(c => new CommentResponse
        {
            Id = c.Id,
            Content = c.Content,
            TaskItemId = c.TaskItemId,
            UserId = c.CreatedByUserId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList()
    });
});

app.MapPost("/minimal/comments", async (CommentCreate dto, ICommentService service) =>
{
    var created = await service.CreateCommentAsync(new Comment
    {
        Content = dto.Content,
        TaskItemId = dto.TaskItemId,
        CreatedByUserId = dto.UserId
    });

    return Results.Ok(new ApiResponse<CommentResponse>
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
});

app.MapPut("/minimal/comments/{id:int}", async (int id, CommentUpdate dto, ICommentService service) =>
{
    var comment = await service.GetCommentByIdAsync(id);
    if (comment == null)
        return Results.NotFound(new ApiResponse<object> { Success = false, Message = "Comment not found" });

    comment.Content = dto.Content;

    var updated = await service.UpdateCommentAsync(comment);

    return Results.Ok(new ApiResponse<CommentResponse>
    {
        Success = true,
        Message = "Comment updated",
        Data = new CommentResponse
        {
            Id = updated!.Id,
            Content = updated.Content,
            TaskItemId = updated.TaskItemId,
            UserId = updated.CreatedByUserId,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        }
    });
});

app.MapDelete("/minimal/comments/{id:int}", async (int id, ICommentService service) =>
{
    if (!await service.DeleteCommentAsync(id))
        return Results.NotFound(new ApiResponse<object> { Success = false, Message = "Comment not found" });

    return Results.NoContent();
});

app.Run();
