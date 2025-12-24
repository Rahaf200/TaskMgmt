using System.ComponentModel.DataAnnotations;
namespace TaskMgmt.DTOs
{
  public class ProjectUpdate
  {
    [Required]
    [MinLength(3)]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
  }
}