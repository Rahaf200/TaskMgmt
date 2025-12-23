namespace TaskMgmt.DTOs
{
  public class CommentResponse
 {
       public int Id { get; set; }
       public string Content { get; set; } = null!;
       public int TaskItemId { get; set; }
       public int UserId { get; set; }
       public DateTime CreatedAt { get; set; }
       public DateTime UpdatedAt { get; set; }
 }
}
