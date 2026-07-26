using FluentAssertions;
using Moq;
using Products.Domain.Entities;
using Products.Domain.Interfaces;
using Products.DTO;
using Products.Services;

namespace Products.Tests.Unit
{
  public class StockMovementServiceTests
  {
    private readonly Mock<IStockMovementRepository> _mockRepo = new();
    private readonly StockMovementService _service;

    public StockMovementServiceTests()
    {
      _service = new StockMovementService(_mockRepo.Object);
    }

    [Fact]
    public async Task CreateAsync_CallsRepositoryCreate_WithCorrectProductIdAndQuantity()
    {
      _mockRepo.Setup(x => x.CreateAsync(It.IsAny<StockMovement>())).ReturnsAsync(10);

      await _service.CreateAsync(new CreateStockMovementDto(100001, 10));

      _mockRepo.Verify(x => x.CreateAsync(It.Is<StockMovement>(m =>
          m.ProductId == 100001 &&
          m.MovementQuantity == 10)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsRunningTotalFromRepository()
    {
      _mockRepo.Setup(x => x.CreateAsync(It.IsAny<StockMovement>())).ReturnsAsync(50);

      var result = await _service.CreateAsync(new CreateStockMovementDto(100001, 10));

      result.Should().Be(50);
    }

    [Fact]
    public async Task CreateAsync_WithPositiveQuantity_ReturnsPositiveTotal()
    {
      _mockRepo.Setup(x => x.CreateAsync(It.IsAny<StockMovement>())).ReturnsAsync(50);

      var result = await _service.CreateAsync(new CreateStockMovementDto(100001, 10));

      result.Should().BePositive();
    }

    [Fact]
    public async Task CreateAsync_WithNegativeQuantity_ReturnsDecreasedTotal()
    {
      _mockRepo.Setup(x => x.CreateAsync(It.IsAny<StockMovement>())).ReturnsAsync(40);

      var result = await _service.CreateAsync(new CreateStockMovementDto(100001, -10));

      result.Should().Be(40);
    }
  }
}
