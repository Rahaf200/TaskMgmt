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

    }
}