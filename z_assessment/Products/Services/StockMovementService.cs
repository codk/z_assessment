using Products.Domain.Interfaces;
using Products.DTO;
using Products.Infrastructure.Repositories;
using Products.interfaces;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Products.Services
{
  public class StockMovementService : IStockMovementService
  {
    protected readonly IStockMovementRepository _stockMovementRepository;
    protected readonly IProductService _productService;

    public StockMovementService(IStockMovementRepository stockMovementRepository, IProductService productService)
    {
      _stockMovementRepository = stockMovementRepository ?? throw new ArgumentNullException(nameof(stockMovementRepository));
      _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    public async Task<int> CreateAsync(CreateStockMovementDto dto)
    {
      var product = await _productService.GetByIdAsync(dto.productId);
      if (product == null)
      {
        throw new InvalidOperationException($"Product with id {dto.productId} not found.");
      }

      await _stockMovementRepository.CreateAsync(new Domain.Entities.StockMovement() { ProductId = dto.productId, StockQuantity = dto.quantity });

      //retreive stock from all movements for the product
      var finalStock = await _stockMovementRepository.GetProductStock(dto.productId);
      

      return finalStock;
    }
  }
}