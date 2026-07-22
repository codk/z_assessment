using Products.Domain.Entities;
using Products.Domain.Interfaces;
using Products.DTO;
using Products.interfaces;

namespace Products.Services
{
  public class ProductService : IProductService
  {
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
      _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
      if (dto == null) throw new ArgumentNullException(nameof(dto));

      var product = new Product
      {
        Name = dto.Name,
        Description = dto.Description,
        Stock = dto.Stock
      };

      await _productRepository.CreateAsync(product);

      return ToDto(product);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
      var products = await _productRepository.GetAllAsync();
      return products.Select(p => ToDto(p)).ToList();
    }

    private static ProductResponseDto ToDto(Product product) => new ProductResponseDto(
      product.Id,
      product.Name,
      product.Description,
      product.Stock
    );
  }
}
