
namespace TaskMgmt.DTOs
{
    public class TaskUpdate
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }

    }
}
