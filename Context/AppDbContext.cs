using Microsoft.EntityFrameworkCore;
using TaskMgmt.Models;

namespace TaskMgmt.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Project>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<TaskItem>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Comment>().HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.CreatedByUser)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.TaskItem)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);


            var seedDate = new DateTime(2024, 1, 1); 

            // USERS
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@taskmgmt.com",
                    PasswordHash = "seeded_password_hash",
                    Role = "Admin",
                    IsDeleted = false,
                    CreatedAt = seedDate,   
                    UpdatedAt = seedDate    
                },
                new User
                {
                    Id = 2,
                    Username = "john",
                    Email = "john@taskmgmt.com",
                    PasswordHash = "seeded_password_hash",
                    Role = "User",
                    IsDeleted = false,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );

            // PROJECTS
            modelBuilder.Entity<Project>().HasData(
                new Project
                {
                    Id = 1,
                    Name = "Task Management System",
                    Description = "Initial seeded project",
                    UserId = 1,
                    IsDeleted = false,
                    CreatedAt = seedDate,   
                    UpdatedAt = seedDate    
                }
            );

            // TASKS
            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem
                {
                    Id = 1,
                    Title = "Design Database",
                    Description = "Create ER diagram",
                    Status = TaskStatus.New,
                    UserId = 1,
                    ProjectId = 1,
                    IsDeleted = false,
                    CreatedAt = seedDate,   
                    UpdatedAt = seedDate    
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Build API",
                    Description = "Implement REST endpoints",
                    Status = TaskStatus.New,
                    UserId = 2,
                    ProjectId = 1,
                    IsDeleted = false,
                    CreatedAt = seedDate,  
                    UpdatedAt = seedDate 
                }
            );

            // COMMENTS
            modelBuilder.Entity<Comment>().HasData(
                new Comment
                {
                    Id = 1,
                    Content = "Database design started",
                    TaskItemId = 1,
                    CreatedByUserId = 1,
                    IsDeleted = false,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IHasTimestamps &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (IHasTimestamps)entry.Entity;
                entity.UpdatedAt = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
