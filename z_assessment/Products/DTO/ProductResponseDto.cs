using System;
using System.Collections.Generic;
using System.Text;

namespace Products.DTO
{
  public record ProductResponseDto(
    int Id,
    string Name,
    string Description,
    int Stock
  );

}
