using Products.DTO;

namespace Products.interfaces
{
  public interface IStockMovementService
  {
    Task<int> CreateAsync(CreateStockMovementDto dto);


  }
}