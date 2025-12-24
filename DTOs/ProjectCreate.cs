using System.ComponentModel.DataAnnotations;
namespace TaskMgmt.DTOs
{

    public class ProjectCreate
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [Range(1, int.MaxValue)]
        public int UserId { get; set; }
    }
}