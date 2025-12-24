using System.ComponentModel.DataAnnotations;
namespace TaskMgmt.DTOs
{
    public class TaskCreate
    {
        [Required]
        [MinLength(3)]
        public string Title { get; set; } = null!; 

        public string? Description { get; set; }

        [Required]
        public TaskStatus Status { get; set; }

        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        [Range(1, int.MaxValue)]
        public int ProjectId { get; set; }

    }
}