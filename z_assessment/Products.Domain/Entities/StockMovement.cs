using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Products.Domain.Entities
{
  public class StockMovement
  {
    [Required]
    public int Id { get; set; }
    public int ProductId { get; set; }

    public virtual Product? Product { get; set; }
    
    [Required]
    public int StockQuantity { get; set; }
    public DateTime CreatedOn { get; set; }
    
  }
}
