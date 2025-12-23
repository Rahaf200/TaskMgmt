namespace TaskMgmt.DTOs
{

    public class ProjectCreate
    {
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int UserId { get; set; }
    }
}