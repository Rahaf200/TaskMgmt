using System.ComponentModel.DataAnnotations;
namespace TaskMgmt.DTOs
{
    public class TaskUpdate
    {
        [Required]
        [MinLength(3)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public TaskStatus Status { get; set; }

    }
}
