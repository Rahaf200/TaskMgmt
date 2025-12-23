namespace TaskMgmt.DTOs
{
    public class TaskResponse
    {
        public int Id { get; set; }
        public string Title { get; set;} = null!;
        public string? Description { get; set; }
         public TaskStatus Status { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } 
    }

} 
