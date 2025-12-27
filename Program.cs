using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskMgmt.Context;
using TaskMgmt.Interfaces;
using TaskMgmt.Services;
using TaskMgmt.Middlewares;
using TaskMgmt.Common;
using TaskMgmt.Models;
using TaskMgmt.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = "TaskMgmtAPI",
        ValidAudience = "TaskMgmtClient",

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("TASKMGMT_SUPER_SECRET_KEY_123")
        )
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// ===================== VALIDATION HELPER =====================
//400 Bad request 
static bool TryValidate<T>(T model, out List<string> errors)
{
    var context = new ValidationContext(model!);
    var results = new List<ValidationResult>();
    var isValid = Validator.TryValidateObject(model!, context, results, true);

    errors = results.Select(r => r.ErrorMessage!).ToList();
    return isValid;
}

// ===================== MIDDLEWARE =====================
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ===================== USERS =========================

app.MapPost("/minimal/users", async (UserCreate dto, IUserService service) =>
{
    if (!TryValidate(dto, out var errors))
    {
        return Results.BadRequest(new ApiResponse<object>
        {
            Success = false,
            Message = "Validation failed",
            Data = errors
        });
    }

    try
    {
        var created = await service.CreateUserAsync(new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
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
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new ApiResponse<object>
        {
            Success = false,
            Message = ex.Message,
            Data = null
        });
    }
});

app.MapPut("/minimal/users/{id:int}", async (int id, UserUpdate dto, IUserService service) =>
{
    if (!TryValidate(dto, out var errors))
    {
        return Results.BadRequest(new ApiResponse<object>
        {
            Success = false,
            Message = "Validation failed",
            Data = errors
        });
    }

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

// ==================== PROJECTS =======================

app.MapPost("/minimal/projects", async (ProjectCreate dto, IProjectService service) =>
{
    if (!TryValidate(dto, out var errors))
    {
        return Results.BadRequest(new ApiResponse<object>
        {
            Success = false,
            Message = "Validation failed",
            Data = errors
        });
    }

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
})
.RequireAuthorization();

app.MapPut("/minimal/projects/{id:int}", async (int id, ProjectUpdate dto, IProjectService service) =>
{
    if (!TryValidate(dto, out var errors))
    {
        return Results.BadRequest(new ApiResponse<object>
        {
            Success = false,
            Message = "Validation failed",
            Data = errors
        });
    }

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
})
.RequireAuthorization();

// ====================== TASKS ========================

app.MapPost("/minimal/projects/{projectId:int}/tasks", async (int projectId, TaskCreate dto, ITaskItemService service) =>
{
    if (!TryValidate(dto, out var errors))
    {
        return Results.BadRequest(new ApiResponse<object>
        {
            Success = false,
            Message = "Validation failed",
            Data = errors
        });
    }

    var created = await service.CreateTaskAsync(new TaskItem
    {
        Title = dto.Title,
        Description = dto.Description,
        Status = dto.Status,
        UserId = dto.UserId,
        ProjectId = projectId
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
})
.RequireAuthorization();

app.MapPut("/minimal/projects/{projectId:int}/tasks/{id:int}",
async (int projectId, int id, TaskUpdate dto, ITaskItemService service) =>
{
    if (!TryValidate(dto, out var errors))
    {
        return Results.BadRequest(new ApiResponse<object>
        {
            Success = false,
            Message = "Validation failed",
            Data = errors
        });
    }

    var task = await service.GetTaskByIdAsync(id);
    if (task == null)
        return Results.NotFound(new ApiResponse<object>
        {
            Success = false,
            Message = "Task not found"
        });
    if (task.ProjectId != projectId)
        return Results.NotFound(new ApiResponse<object>
        {
            Success = false,
            Message = "Task does not belong to this project"
        });

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
})
.RequireAuthorization();


// ===================== COMMENTS ======================

app.MapPost("/minimal/tasks/{taskId:int}/comments", async (int taskId, CommentCreate dto, ICommentService service) =>
{
    if (!TryValidate(dto, out var errors))
    {
        return Results.BadRequest(new ApiResponse<object>
        {
            Success = false,
            Message = "Validation failed",
            Data = errors
        });
    }

    var created = await service.CreateCommentAsync(new Comment
    {
        Content = dto.Content,
        TaskItemId = taskId,
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
})
.RequireAuthorization();

app.MapPut("/minimal/tasks/{taskId:int}/comments/{id:int}", async (int taskId, int id, CommentUpdate dto, ICommentService service) =>
{
    if (!TryValidate(dto, out var errors))
    {
        return Results.BadRequest(new ApiResponse<object>
        {
            Success = false,
            Message = "Validation failed",
            Data = errors
        });
    }

    var comment = await service.GetCommentByIdAsync(id);
    if (comment == null)
        return Results.NotFound(new ApiResponse<object> { Success = false, Message = "Comment not found" });

    if (comment.TaskItemId != taskId)
        return Results.NotFound(new ApiResponse<object>
        {
            Success = false,
            Message = "Comment does not belong to this task"
        });

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
})
.RequireAuthorization();

app.Run();
