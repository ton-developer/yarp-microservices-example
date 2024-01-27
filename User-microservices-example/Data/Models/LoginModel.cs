using System.ComponentModel.DataAnnotations;

namespace User_microservices_example.Data.Models;

public class LoginModel
{
    [Required(ErrorMessage = "User Name is required")]
    [EmailAddress]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}