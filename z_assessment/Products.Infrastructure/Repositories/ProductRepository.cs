using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;
using Products.Domain.Interfaces;

namespace Products.Infrastructure.Repositories
{
  public class ProductRepository : BaseRepo<Product>, IProductRepository
  {
    protected readonly AppDBContext _context;

    public ProductRepository(AppDBContext context) : base(context)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
      return await _context.Products
        .Include(p => p.StockMovements.Where(m => m.IsActive))
        .ToListAsync();
    }



    public async Task<IEnumerable<Product>> ProductSearch(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        return await _context.Products
           .Include(p => p.StockMovements.Where(m => m.IsActive))
           .ToListAsync();

      return await _context.Products
         .Include(p => p.StockMovements.Where(m => m.IsActive))                           
         .Where(p => EF.Functions.Like(p.Name, $"%{name}%"))
         .ToListAsync();
    }

    public async Task<IEnumerable<Product>> ProductSearchByStock(int min, int max)
    {
      if (min > max) (min, max) = (max, min);
      return await _context.Products
         .Include(p => p.StockMovements.Where(m => m.IsActive))
         .Where(p => p.Stock >= min && p.Stock <= max)
         .ToListAsync();
    }
  }
}