using System.Collections.Generic;

namespace TaskMgmt.Models
{
    public class TaskItem : IHasTimestamps
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.New;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public List<Comment> Comments { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

 
    }
}