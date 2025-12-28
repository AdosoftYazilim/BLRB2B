using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLRB2B.Domain.Entities;

[Table("Customers")]
public class Customer : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContactName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(20)]
    public string? TaxNumber { get; set; }

    [MaxLength(20)]
    public string? TaxOffice { get; set; }

    public bool IsActive { get; set; } = true;

    public decimal CreditLimit { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountRate { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}