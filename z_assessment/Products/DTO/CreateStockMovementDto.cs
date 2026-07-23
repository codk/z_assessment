using Npgsql.PostgresTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Products.DTO
{
  public record CreateStockMovementDto(int productId, int quantity);
}
