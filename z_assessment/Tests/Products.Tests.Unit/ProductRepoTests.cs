using FluentAssertions;
using Moq;
using Products.Domain.Entities;
using Products.Domain.Interfaces;
using Products.DTO;
using Products.Services;

namespace Products.Tests.Unit
{
  public class ProductRepoTests
  {
    private Mock<IProductRepository> _mockProductRepo = new();
    private Mock<IStockMovementRepository> _mockStockMovementRepo = new();
    protected readonly ProductService _mockProductService;
    protected readonly StockMovementService _mockStockMovementService;

    public ProductRepoTests()
    {
      _mockProductService = new ProductService(_mockProductRepo.Object);
      _mockStockMovementService = new StockMovementService(_mockStockMovementRepo.Object, _mockProductService);
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
      var product = CreateProduct(100001, "Alpha");
      _mockProductRepo.Setup(r => r.GetByIdAsync(100001)).ReturnsAsync((Product?)product);

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
                        CreateProduct(100001, "Alpha"),
                        CreateProduct(100002, "Beta")
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
                        new Product(){ Id = 100001, Name = "A", Stock=5},
                        new Product(){ Id = 100002, Name = "A", Stock=20}
                    };


      _mockProductRepo.Setup(r => r.ProductSearchByStock(1, 20)).ReturnsAsync(products);

      var result = await _mockProductService.GetByStockAsync(1, 20);

      result.Should().HaveCount(2);
      result.All(p => p.Stock >= 1 && p.Stock <= 20).Should().BeTrue();
    }

    [Fact]
    public async Task Create_ReturnsCreatedProduct_AndChecksId()
    {
      var dto = new CreateProductDto("Product 1", "Description for product 1");
      _mockProductRepo.Setup(x => x.CreateAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => { p.Id = 1000000; return p.Id; });

      var result = await _mockProductService.CreateAsync(dto);
      result.Id.Should().Be(1000000);
    }


    private static Product CreateProduct(int id, string name, string description = default) => new Product { Id = id, Name = name, Description = description };
  }
}