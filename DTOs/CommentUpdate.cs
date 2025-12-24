using System.ComponentModel.DataAnnotations;
namespace TaskMgmt.DTOs
{
 public class CommentUpdate
 {
   [Required]
   [MinLength(1)]
    public string Content { get; set; } = null!;
 }
}