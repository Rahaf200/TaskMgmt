using System.Collections.Generic;

namespace TaskMgmt.Models
{
    public class User : IHasTimestamps
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "User";

        public List<TaskItem> Tasks { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public List<Project> Projects { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}