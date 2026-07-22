using Products.DTO;


namespace Products.interfaces
{
  internal interface IProductService
  {

    Task<ProductResponseDto> CreateAsync(CreateProductDto dto);

    Task<IEnumerable<ProductResponseDto>> GetAllAsync();
  }
}
