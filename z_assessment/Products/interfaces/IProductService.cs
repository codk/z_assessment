using Products.DTO;

namespace Products.interfaces
{
  public interface IProductService
  {
    Task<ProductResponseDto> CreateAsync(CreateProductDto dto);

    Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto);

    Task<IEnumerable<ProductResponseDto>> GetAllAsync();

    Task<ProductResponseDto?> GetByIdAsync(int id);

    Task<IEnumerable<ProductResponseDto>> GetByNameAsync(string name);

    Task<IEnumerable<ProductResponseDto>> GetByStockAsync(int stockMin, int stockMax);

    Task<bool> DeleteAsync(int id);
  }
}