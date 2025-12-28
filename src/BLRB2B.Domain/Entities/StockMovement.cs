using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLRB2B.Domain.Entities;

/// <summary>
/// Represents a stock movement in the B2B e-commerce system
/// </summary>
[Table("StockMovements")]
public class StockMovement : BaseEntity
{
    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public int Quantity { get; set; }

    [Required]
    [MaxLength(20)]
    public string MovementType { get; set; } = StockMovementType.In;

    public int? OrderId { get; set; }

    public DateTime MovementDate { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey(nameof(ProductId))]
    public virtual Product? Product { get; set; }

    [ForeignKey(nameof(WarehouseId))]
    public virtual Warehouse? Warehouse { get; set; }

    [ForeignKey(nameof(OrderId))]
    public virtual Order? Order { get; set; }
}

/// <summary>
/// Stock movement type enumeration
/// </summary>
public static class StockMovementType
{
    public const string In = "In";
    public const string Out = "Out";
    public const string Reservation = "Reservation";
}
