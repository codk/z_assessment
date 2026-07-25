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
      await using var tx = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

      if (!await _context.Products.AnyAsync(p => p.Id == entity.ProductId))
        throw new InvalidOperationException($"Product {entity.ProductId} not found.");

      var current = await _context.StockMovements
         .Where(m => m.ProductId == entity.ProductId && m.IsActive)
         .FirstOrDefaultAsync();

      var currentTotal = current?.RunningTotal ?? 0;
      var newTotal = currentTotal + entity.MovementQuantity;

      if (newTotal < 0)
        throw new InvalidOperationException($"No stock available. Current {currentTotal}, Requested:{Math.Abs(entity.MovementQuantity)}.");

      if (current is not null)
        current.IsActive = false;

      await _context.StockMovements.AddAsync(new
        StockMovement
      {
        ProductId = entity.ProductId,
        MovementQuantity = entity.MovementQuantity,
        RunningTotal = newTotal,
        IsActive = true
      });

      await _context.SaveChangesAsync();
      await tx.CommitAsync();

      return newTotal;
    }

    public async Task<int> GetProductStock(int productId)
    {
      var stock = await _context.StockMovements
          .Where(x => x.ProductId == productId && x.IsActive)
          .FirstOrDefaultAsync();

      return stock?.RunningTotal ?? 0;
    }
  }
}