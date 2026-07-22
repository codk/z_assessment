using Products.Domain.Entities;

namespace Products.Domain.Interfaces
{
  internal interface IProductRepository : IBaseRepo<Product>
  {
      public Task<bool> DecrementStock(int productId, int quantity);

      public Task<bool> IncrementStock(int productId, int quantity);
      public Task<IEnumerable<Product>> ProductSearch(string name);

      public Task<IEnumerable<Product>> ProductSearchByStock(int min, int max);

  }
}
