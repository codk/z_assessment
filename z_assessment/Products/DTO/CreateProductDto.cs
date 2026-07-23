using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Products.DTO
{
  public record CreateProductDto(
    [Required]
    string Name,
    string Description,
    [Required]
    int Stock
  );

}
