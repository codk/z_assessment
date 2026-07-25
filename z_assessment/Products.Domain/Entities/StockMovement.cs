using System.ComponentModel.DataAnnotations;

namespace Products.Domain.Entities
{
  public class StockMovement
  {
    [Required]
    public int Id { get; set; }

    public int ProductId { get; set; }

    public virtual Product? Product { get; set; }

    /// <summary>
    /// Value to add\remove over current stock <code>RunnigTotal</code> and <code>IsActive = True</code>
    /// </summary>
    [Required]
    public int MovementQuantity { get; set; }

    /// <summary>
    /// Current product stock
    /// </summary>
    [Required]
    public int RunningTotal { get; set; } = 0;

    [Required]
    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }
  }
}