using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
      var product = await _productRepository.GetByIdAsync(id);
      return product is null ? null : ToDto(product);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetByNameAsync(string name)
    {
      if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
      var products = await _productRepository.ProductSearch(name);
      return products?.Select(ToDto) ?? Enumerable.Empty<ProductResponseDto>();
    }

    public async Task<IEnumerable<ProductResponseDto>> GetByStockAsync(int stockMin, int stockMax)
    {
      var products = await _productRepository.ProductSearchByStock(stockMin, stockMax);
      return products?.Select(ToDto) ?? Enumerable.Empty<ProductResponseDto>();
    }

    #region stock operations
    public async Task<bool> StockIncrementAsync(int productId, int incrementBy)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) return false;

        product.Stock += incrementBy;
        await _productRepository.UpdateAsync(product);
        return true;
    }


    public async Task<bool> StockDecrementAsync(int productId, int incrementBy)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) return false;
        
        product.Stock -= incrementBy;
        await _productRepository.UpdateAsync(product);
        return true;
    }




    #endregion



    #region mapping operations
    private static ProductResponseDto ToDto(Product product) => new ProductResponseDto(
      product.Id,
      product.Name,
      product.Description,
      product.Stock
    );


    #endregion
  }
}
