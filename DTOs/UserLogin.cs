using System.ComponentModel.DataAnnotations;

namespace TaskMgmt.DTOs;

public class UserLogin
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
