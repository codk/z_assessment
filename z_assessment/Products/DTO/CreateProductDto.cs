using System.ComponentModel.DataAnnotations;

namespace Products.DTO
{
  public record CreateProductDto(
    [Required]
    string Name,
    string Description

  );
}