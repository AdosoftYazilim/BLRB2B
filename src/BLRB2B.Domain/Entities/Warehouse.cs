using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLRB2B.Domain.Entities;

/// <summary>
/// Represents a warehouse in the B2B e-commerce system
/// </summary>
[Table("Warehouses")]
public class Warehouse : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;
}
