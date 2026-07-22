using System.ComponentModel.DataAnnotations;

namespace Products.Domain.Entities
{
  public class Product
  {
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    public int Stock { get; set; } = 0;
    public DateTime CreatedOn {  get; set; }
    public DateTime UpdatedOn { get; set; }

  }
}
