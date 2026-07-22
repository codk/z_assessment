using System;
using System.Collections.Generic;
using System.Text;

namespace Products.DTO
{
  public record CreateProductDto(
    string Name,
    string Description,
    int Stock
  );

}
