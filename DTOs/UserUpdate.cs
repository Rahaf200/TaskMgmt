using System.ComponentModel.DataAnnotations;
namespace TaskMgmt.DTOs
{
    public class UserUpdate
    {
        [Required]
        [MinLength(3)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!; 

    }
}
