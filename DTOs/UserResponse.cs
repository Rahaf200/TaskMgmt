namespace TaskMgmt.DTOs
{
    public class UserResponse
    {
        public int Id { get; set;}
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
