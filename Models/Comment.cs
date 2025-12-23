
using TaskMgmt.Models;

namespace TaskMgmt.Models
{
    public class Comment : IHasTimestamps
    {
        public int Id { get; set; }
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = null!; 
        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int CreatedByUserId { get; set; }
         
        public User CreatedByUser { get; set; } = null!;

    }
}