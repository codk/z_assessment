using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Products.DTO
{
  public record UpdateProductDto(
    [Required]
    string Name,
    string Description//,
   // int Stock
  );
  
}
