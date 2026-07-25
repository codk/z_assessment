using System.ComponentModel.DataAnnotations;

namespace Products.DTO
{
  public record UpdateProductDto(
    [Required]
    string Name,
    string Description
  );
}