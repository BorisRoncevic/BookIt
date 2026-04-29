using System.ComponentModel.DataAnnotations;

namespace Auth.Service.Models;

public class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";
}
