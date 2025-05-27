// Extending existing RegisterUserDto with password
using System.ComponentModel.DataAnnotations;

public class RegisterUserDto
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters allowed")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only letters allowed")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [RegularExpression(@"^[a-z0-9]+@(gmail\.com|credwise\.com)$",
        ErrorMessage = "Only gmail.com or credwise.com domains allowed")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid Indian phone number")]
    public string PhoneNumber { get; set; }

    [RegularExpression(@"^(Customer|Admin)$", ErrorMessage = "Must be Customer or Admin")]
    public string Role { get; set; } = "Customer";

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")]
    public string Password { get; set; }

}

public class UserResponseDto
{
    public int UserId { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public override string ToString() =>
      $"User {UserId}: {Email} (Active: {IsActive})";
}

public class UpdateUserDto
{
    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    [Required]
    [Phone]
    public string PhoneNumber { get; set; }

    
    public string Role { get; set; }
}

public class TestLoginDto
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}