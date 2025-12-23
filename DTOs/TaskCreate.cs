
namespace TaskMgmt.DTOs
{
    public class TaskCreate
    {
        public string Title { get; set; } = null!; 
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }

    }
}