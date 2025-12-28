using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLRB2B.Domain.Entities;

/// <summary>
/// Represents a user in the B2B e-commerce system
/// </summary>
[Table("Users")]
public class User : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? FirstName { get; set; }

    [MaxLength(50)]
    public string? LastName { get; set; }

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = UserRole.User;

    public int? CustomerId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginDate { get; set; }

    // Navigation properties
    [ForeignKey(nameof(CustomerId))]
    public virtual Customer? Customer { get; set; }
}

/// <summary>
/// User role enumeration
/// </summary>
public static class UserRole
{
    public const string Admin = "Admin";
    public const string User = "User";
}
