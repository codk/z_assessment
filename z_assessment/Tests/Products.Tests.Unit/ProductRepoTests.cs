using Moq;
using FluentAssertions;
using Products.Domain.Entities;
using Products.Domain.Interfaces;
using Products.Services;
using Products.DTO;

namespace Products.Tests.Unit
{
  public class ProductRepoTests
  {
    Mock<IProductRepository> _mockProductRepo = new();
    protected readonly ProductService _mockProductService;

    public ProductRepoTests()
    {   
      _mockProductService = new ProductService(_mockProductRepo.Object);
    }

    [Fact]
    public async Task GetAllProducts()
    {
      var products=new List<Product>
      {
        new Product { Id = 1, Name = "Product 1", Stock = 100 },
        new Product { Id = 2, Name = "Product 2", Stock = 200 }
      };

      _mockProductRepo.Setup(x=>x.GetAllAsync()).ReturnsAsync(products);

      var result = await _mockProductService.GetAllAsync();
      result.Should().HaveCount(2);
      result.Select(t=>t.Name).Should().Contain(new List<string> { "Product 1", "Product 2" });
    }

    [Fact]
    public async Task Create_ReturnsCreatedProduct_AndChecksId()
    {
      var dto = new CreateProductDto("Product 1", "Description for product 1", 99);
      _mockProductRepo.Setup(x => x.CreateAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => { p.Id = 1000000; return p.Id; });

      var result = await _mockProductService.CreateAsync(dto);
      result.Id.Should().Be(1000000);
    }




  }
}
