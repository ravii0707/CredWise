// Extending existing RegisterUserDto with password
using System.ComponentModel.DataAnnotations;

public class RegisterUserDto
{
    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    [EmailAddress, Required]
    public string? Email { get; set; }

    [Required]
    [Phone]
    public string? PhoneNumber { get; set; }

    [Required]
    public string? Role { get; set; } // Admin or Customer

    [Required, MinLength(8)] // Change from 6 to 8 if needed
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password must contain uppercase, lowercase, number, and special character")]
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