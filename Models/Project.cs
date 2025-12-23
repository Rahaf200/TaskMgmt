using System.Collections.Generic;

namespace TaskMgmt.Models
{
    public class Project : IHasTimestamps
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public List<TaskItem> Tasks { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}