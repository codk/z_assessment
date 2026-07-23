using Moq;
using FluentAssertions;
using Products.Domain.Entities;
using Products.Domain.Interfaces;
using Products.Services;
using Products.DTO;
using System.Runtime.CompilerServices;


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
      var products = new List<Product>
      {
        new Product { Id = 1, Name = "Product 1", Stock = 100 },
        new Product { Id = 2, Name = "Product 2", Stock = 200 }
      };

      _mockProductRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

      var result = await _mockProductService.GetAllAsync();
      result.Should().HaveCount(2);
      result.Select(t => t.Name).Should().Contain(new string[] { "Product 1", "Product 2" });
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsProduct_WhenExists()
    {
      var product = CreateProduct(100001, "Alpha", 5);
      _mockProductRepo.Setup(r => r.GetByIdAsync(100001)).ReturnsAsync(product);

      var result = await _mockProductService.GetByIdAsync(100001);

      result.Should().NotBeNull();
      result!.Id.Should().Be(100001);
      result.Name.Should().Be("Alpha");
    }

    [Fact]
    public async Task GetByNameAsync_ReturnsProductsWithMatchingName()
    {
      var products = new List<Product>
        {
            CreateProduct(100001, "Alpha", 5),
            CreateProduct(100002, "Beta", 15)
        };
      _mockProductRepo.Setup(r => r.ProductSearch("Alpha")).ReturnsAsync(products.Where(p => p.Name == "Alpha"));
      var result = await _mockProductService.GetByNameAsync("Alpha");
      result.Should().HaveCount(1);
      result.First().Name.Should().Be("Alpha");
    }
    //stock
    [Fact]
    public async Task GetByStockRangeAsync_ReturnsProductsInRange()
    {
      var products = new List<Product>
        {
            CreateProduct(100001, "A", 5),
            CreateProduct(100002, "B", 15)
        };
      _mockProductRepo.Setup(r => r.ProductSearchByStock(1, 20)).ReturnsAsync(products);

      var result = await _mockProductService.GetByStockAsync(1, 20);

      result.Should().HaveCount(2);
      result.All(p => p.Stock >= 1 && p.Stock <= 20).Should().BeTrue();
    }



    [Fact]
    public async Task Create_ReturnsCreatedProduct_AndChecksId()
    {
      var dto = new CreateProductDto("Product 1", "Description for product 1", 99);
      _mockProductRepo.Setup(x => x.CreateAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => { p.Id = 1000000; return p.Id; });

      var result = await _mockProductService.CreateAsync(dto);
      result.Id.Should().Be(1000000);
    }

    static Product CreateProduct(int id, string name, int stock, string description=default) => new Product{Id=id, Name=name, Stock=stock, Description=description};




  }
}
