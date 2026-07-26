using Products.Domain.Entities;

namespace Products.Domain.Interfaces
{
  public interface IStockMovementRepository : IBaseRepo<StockMovement>
  {
    Task<int> GetProductStock(int productId);
  }
}