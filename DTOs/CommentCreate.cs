using System.ComponentModel.DataAnnotations;
namespace TaskMgmt.DTOs
{ 
  public class CommentCreate
  {
    [Required]
    [MinLength(1)]
    public string Content { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int TaskItemId { get; set; }

    [Range(1, int.MaxValue)]
    public int UserId { get; set; }
  }
}