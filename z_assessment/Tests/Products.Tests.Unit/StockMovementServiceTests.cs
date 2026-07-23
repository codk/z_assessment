using FluentAssertions;
using Moq;
using Products.Domain.Entities;
using Products.Domain.Interfaces;
using Products.DTO;
using Products.interfaces;
using Products.Services;

namespace Products.Tests.Unit
{
  public class StockMovementServiceTests
  {
    private readonly Mock<IStockMovementRepository> _mockRepo = new();
    private readonly Mock<IProductService> _mockProductService = new();
    private readonly StockMovementService _service;

    public StockMovementServiceTests()
    {
      _service = new StockMovementService(_mockRepo.Object, _mockProductService.Object);
    }

    [Fact]
    public async Task CreateAsync_ThrowsInvalidOperationException_WhenProductNotFound()
    {
      _mockProductService.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((ProductResponseDto?)null);

      var act = async () => await _service.CreateAsync(new CreateStockMovementDto(99, 10));

      await act.Should().ThrowAsync<InvalidOperationException>()
          .WithMessage("*99*");
    }

    [Fact]
    public async Task CreateAsync_CallsRepositoryCreate_WithCorrectProductIdAndQuantity()
    {
      _mockProductService.Setup(x => x.GetByIdAsync(100001)).ReturnsAsync(new ProductResponseDto(100001, "Alpha", null, 0));
      _mockRepo.Setup(x => x.CreateAsync(It.IsAny<StockMovement>())).ReturnsAsync(1);
      _mockRepo.Setup(x => x.GetProductStock(100001)).ReturnsAsync(10);

      await _service.CreateAsync(new CreateStockMovementDto(100001, 10));

      _mockRepo.Verify(x => x.CreateAsync(It.Is<StockMovement>(m =>
          m.ProductId == 100001 &&
          m.StockQuantity == 10)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsStockFromGetProductStock()
    {
      _mockProductService.Setup(x => x.GetByIdAsync(100001)).ReturnsAsync(new ProductResponseDto(100001, "Alpha", null, 0));
      _mockRepo.Setup(x => x.CreateAsync(It.IsAny<StockMovement>())).ReturnsAsync(1);
      _mockRepo.Setup(x => x.GetProductStock(100001)).ReturnsAsync(50);

      var result = await _service.CreateAsync(new CreateStockMovementDto(100001, 10));

      result.Should().Be(50);
    }

    [Fact]
    public async Task CreateAsync_WithPositiveQuantity_ReturnsIncreasedStock()
    {
      _mockProductService.Setup(x => x.GetByIdAsync(100001)).ReturnsAsync(new ProductResponseDto(100001, "Alpha", null, 40));
      _mockRepo.Setup(x => x.CreateAsync(It.IsAny<StockMovement>())).ReturnsAsync(1);
      _mockRepo.Setup(x => x.GetProductStock(100001)).ReturnsAsync(50);

      var result = await _service.CreateAsync(new CreateStockMovementDto(100001, 10));

      result.Should().Be(50);
      result.Should().BeGreaterThan(40);
    }

    [Fact]
    public async Task CreateAsync_WithNegativeQuantity_ReturnsDecreasedStock()
    {
      _mockProductService.Setup(x => x.GetByIdAsync(100001)).ReturnsAsync(new ProductResponseDto(100001, "Alpha", null, 50));
      _mockRepo.Setup(x => x.CreateAsync(It.IsAny<StockMovement>())).ReturnsAsync(1);
      _mockRepo.Setup(x => x.GetProductStock(100001)).ReturnsAsync(40);

      var result = await _service.CreateAsync(new CreateStockMovementDto(100001, -10));

      result.Should().Be(40);
      result.Should().BeLessThan(50);
    }

    [Fact]
    public async Task CreateAsync_GetProductStock_IsCalledAfterCreate()
    {
      var callOrder = new List<string>();

      _mockProductService.Setup(x => x.GetByIdAsync(100001)).ReturnsAsync(new ProductResponseDto(100001, "Alpha", null, 0));
      _mockRepo.Setup(x => x.CreateAsync(It.IsAny<StockMovement>()))
          .Callback(() => callOrder.Add("create"))
          .ReturnsAsync(1);
      _mockRepo.Setup(x => x.GetProductStock(100001))
          .Callback(() => callOrder.Add("getstock"))
          .ReturnsAsync(10);

      await _service.CreateAsync(new CreateStockMovementDto(100001, 10));

      callOrder.Should().ContainInOrder("create", "getstock");
    }
  }
}
