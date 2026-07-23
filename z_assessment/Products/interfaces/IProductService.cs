using Products.DTO;


namespace Products.interfaces
{
  public interface IProductService
  {

    Task<ProductResponseDto> CreateAsync(CreateProductDto dto);

    Task<IEnumerable<ProductResponseDto>> GetAllAsync();
  }
}
