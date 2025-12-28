using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLRB2B.Domain.Entities;

/// <summary>
/// Represents a customer group with specific discount rates
/// </summary>
[Table("CustomerGroups")]
public class CustomerGroup : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountPercent { get; set; }

    // Navigation properties
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
