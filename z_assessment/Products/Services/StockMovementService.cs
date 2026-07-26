using Microsoft.EntityFrameworkCore;
using Products.Domain.Interfaces;
using Products.DTO;
using Products.interfaces;

namespace Products.Services
{
  public class StockMovementService : IStockMovementService
  {
    protected readonly IStockMovementRepository _stockMovementRepository;

    public StockMovementService(IStockMovementRepository stockMovementRepository)
    {
      _stockMovementRepository = stockMovementRepository ?? throw new ArgumentNullException(nameof(stockMovementRepository));
    }

    public async Task<int> CreateAsync(CreateStockMovementDto dto)
    {

      var stock = await _stockMovementRepository.CreateAsync(new Domain.Entities.StockMovement() { ProductId = dto.productId, MovementQuantity = dto.quantity });

      return stock;
      
    }
  }
}