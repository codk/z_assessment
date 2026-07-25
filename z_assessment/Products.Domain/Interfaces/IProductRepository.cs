using Products.Domain.Entities;

namespace Products.Domain.Interfaces
{
  public interface IProductRepository : IBaseRepo<Product>
  {

    public Task<IEnumerable<Product>> ProductSearch(string name);

    public Task<IEnumerable<Product>> ProductSearchByStock(int min, int max);
  }
}