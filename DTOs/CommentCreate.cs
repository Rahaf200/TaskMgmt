namespace TaskMgmt.DTOs
{ 
  public class CommentCreate
  {
    public string Content { get; set; } = null!;
    public int TaskItemId { get; set; }
    public int UserId { get; set; }
  }
}

