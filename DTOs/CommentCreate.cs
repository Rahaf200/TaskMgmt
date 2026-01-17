using System.ComponentModel.DataAnnotations;
namespace TaskMgmt.DTOs
{ 
  public class CommentCreate
  {
    [Required]
    [MinLength(1)]
    public string Content { get; set; } = null!;
    
  }
}