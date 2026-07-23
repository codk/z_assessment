using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;
using Products.Domain.Interfaces;

namespace Products.Infrastructure.Repositories
{
  public class StockMovementRepository : BaseRepo<StockMovement>, IStockMovementRepository
  {
    protected readonly AppDBContext _context;

    public StockMovementRepository(AppDBContext context) : base(context)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public override async Task<int> CreateAsync(StockMovement entity)
    {
      var product = await _context.Products.FindAsync(entity.ProductId)
          ?? throw new InvalidOperationException($"Product {entity.ProductId} not found.");

      // get absolute stock for the product -  sum of all movements
      var stockActual = await GetProductStock(entity.ProductId);

      await _context.StockMovements.AddAsync(entity);
      product.Stock = stockActual + entity.StockQuantity;

      return await _context.SaveChangesAsync();
    }

    // Source of truth - get the stock for a product by summing all movements
    public async Task<int> GetProductStock(int productId)
    {
      return await _context.StockMovements
          .Where(x => x.ProductId == productId)
          .SumAsync(x => x.StockQuantity);
    }
  }
}
