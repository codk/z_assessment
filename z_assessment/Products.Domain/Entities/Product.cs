using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Products.Domain.Entities
{
  public class Product
  {
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }

    [NotMapped]
    public int Stock => StockMovements.FirstOrDefault(sm => sm.IsActive)?.RunningTotal ?? 0;

    public ICollection<StockMovement> StockMovements { get; set; } = [];
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
  }
}