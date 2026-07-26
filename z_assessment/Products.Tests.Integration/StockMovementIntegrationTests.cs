using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;
using Products.Infrastructure;
using Products.Infrastructure.Repositories;
using Testcontainers.PostgreSql;
using Xunit;

namespace Products.Tests.Integration
{
  public class StockMovementIntegrationTests : IAsyncLifetime
  {
    #region container specs

    // private readonly DatabaseFixture _fixture;

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
          .WithImage("postgres:16-alpine")
          .Build();

    public AppDBContext CreateContext()
    {
      var options = new DbContextOptionsBuilder<AppDBContext>()
          .UseNpgsql(_container.GetConnectionString())
          .Options;
      return new AppDBContext(options);
    }

    public async Task InitializeAsync()
    {
      await _container.StartAsync();
      await using var ctx = CreateContext();
      await ctx.Database.MigrateAsync();
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();

    #endregion container specs

    [Fact]
    public async Task AddStock_CreatesActiveMovement_WithCorrectRunningTotal()
    {
      await using var ctx = CreateContext();
      // Arrange - create a product that the movement will attach to
      ctx.Products.Add(new Product { Name = "Test Product A" });
      await ctx.SaveChangesAsync();
      var productId = ctx.Products.Single().Id;

      var repo = new StockMovementRepository(ctx);

      // Act - add stock
      var runningTotal = await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = 10 });

      // Assert - returned running total is correct
      runningTotal.Should().Be(10);

      // Assert - exactly one active movement exists for the product and has the expected running total
      var activeMovements = await ctx.StockMovements.Where(m => m.ProductId == productId && m.IsActive).ToListAsync();
      activeMovements.Should().HaveCount(1);
      activeMovements.Single().RunningTotal.Should().Be(10);
    }

    [Fact]
    public async Task AddStock_DeactivatesPreviousMovement_WhenOneExists()
    {
      await using var ctx = CreateContext();
      ctx.Products.Add(new Product { Name = "Test Product B" });
      await ctx.SaveChangesAsync();
      var productId = ctx.Products.Single(p => p.Name == "Test Product B").Id;

      var repo = new StockMovementRepository(ctx);

      // First add
      var firstTotal = await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = 10 });
      firstTotal.Should().Be(10);

      // Second add
      var secondTotal = await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = 5 });
      secondTotal.Should().Be(15);

      // Verify previous movement was deactivated and new one is active
      var movements = await ctx.StockMovements.Where(m => m.ProductId == productId).OrderBy(m => m.Id).ToListAsync();
      movements.Count.Should().Be(2);

      var previous = movements.First();
      var current = movements.Last();

      previous.IsActive.Should().BeFalse();
      previous.RunningTotal.Should().Be(10);

      current.IsActive.Should().BeTrue();
      current.RunningTotal.Should().Be(15);
    }

    [Fact]
    public async Task AddStock_OnlyOneActiveMovement_PerProduct()
    {
      await using var ctx = CreateContext();
      ctx.Products.Add(new Product { Name = "Test Product C" });
      await ctx.SaveChangesAsync();
      var productId = ctx.Products.Single(p => p.Name == "Test Product C").Id;

      var repo = new StockMovementRepository(ctx);

      await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = 5 });
      await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = 3 });
      await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = -2 });

      var activeCount = await ctx.StockMovements.Where(m => m.ProductId == productId && m.IsActive).CountAsync();
      activeCount.Should().Be(1);
    }

    [Fact]
    public async Task DecrementStock_ReturnsCorrectRunningTotal()
    {
      await using var ctx = CreateContext();
      ctx.Products.Add(new Product { Name = "Test Product D" });
      await ctx.SaveChangesAsync();
      var productId = ctx.Products.Single(p => p.Name == "Test Product D").Id;

      var repo = new StockMovementRepository(ctx);

      await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = 10 });

      var newTotal = await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = -3 });
      newTotal.Should().Be(7);

      var active = await ctx.StockMovements.Where(m => m.ProductId == productId && m.IsActive).SingleAsync();
      active.RunningTotal.Should().Be(7);
    }

    [Fact]
    public async Task DecrementStock_ThrowsInvalidOperationException_WhenStockInsufficient()
    {
      await using var ctx = CreateContext();
      ctx.Products.Add(new Product { Name = "Test Product E" });
      await ctx.SaveChangesAsync();
      var productId = ctx.Products.Single(p => p.Name == "Test Product E").Id;

      var repo = new StockMovementRepository(ctx);

      await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = 2 });

      Func<Task> act = async () => await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = -5 });

      await act.Should().ThrowAsync<InvalidOperationException>()
        .WithMessage("*No stock available*");
    }

    [Fact]
    public async Task DecrementStock_Succeeds_WhenStockReachesExactlyZero()
    {
      await using var ctx = CreateContext();
      ctx.Products.Add(new Product { Name = "Test Product F" });
      await ctx.SaveChangesAsync();
      var productId = ctx.Products.Single(p => p.Name == "Test Product F").Id;

      var repo = new StockMovementRepository(ctx);

      await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = 3 });

      var newTotal = await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = -3 });
      newTotal.Should().Be(0);

      var active = await ctx.StockMovements.Where(m => m.ProductId == productId && m.IsActive).SingleAsync();
      active.RunningTotal.Should().Be(0);
      active.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task MultipleMovements_RunningTotalReflectsAllOperations()
    {
      await using var ctx = CreateContext();
      ctx.Products.Add(new Product { Name = "Test Product G" });
      await ctx.SaveChangesAsync();
      var productId = ctx.Products.Single(p => p.Name == "Test Product G").Id;

      var repo = new StockMovementRepository(ctx);

      var t1 = await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = 10 });
      t1.Should().Be(10);

      var t2 = await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = -4 });
      t2.Should().Be(6);

      var t3 = await repo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = 2 });
      t3.Should().Be(8);

      var all = await ctx.StockMovements.Where(m => m.ProductId == productId).OrderBy(m => m.Id).ToListAsync();
      all.Select(m => m.RunningTotal).Should().ContainInOrder(new[] { 10, 6, 8 });
      all.Count.Should().Be(3);
      all.Count(m => m.IsActive).Should().Be(1);
      all.Last().RunningTotal.Should().Be(8);
    }

    [Fact]
    public async Task ConcurrentStockUpdates_RunningTotalIsConsistent()
    {
      const int parallelTasks = 10;

      // Create product
      await using var ctx = CreateContext();
      ctx.Products.Add(new Product { Name = "Test Product H" });
      await ctx.SaveChangesAsync();
      var productId = ctx.Products.Single(p => p.Name == "Test Product H").Id;

      // Run parallel increments, each in its own context/repo
      var tasks = Enumerable.Range(0, parallelTasks).Select(_ => Task.Run(async () =>
      {
        var attempts = 0;
        while (true)
        {
          attempts++;
          try
          {
            await using var localCtx = CreateContext();
            var localRepo = new StockMovementRepository(localCtx);
            await localRepo.CreateAsync(new StockMovement { ProductId = productId, MovementQuantity = 1 });
            break;
          }
          catch (Exception)
          {
            if (attempts >= 5) throw;
            await Task.Delay(50);
          }
        }
      })).ToArray();

      await Task.WhenAll(tasks);

      // Verify final total
      await using var verifyCtx = CreateContext();
      var verifyRepo = new StockMovementRepository(verifyCtx);
      var final = await verifyRepo.GetProductStock(productId);
      final.Should().Be(parallelTasks);
    }

    [Fact]
    public async Task AddStock_ThrowsInvalidOperationException_WhenProductDoesNotExist()
    {
      await using var ctx = CreateContext();
      var repo = new StockMovementRepository(ctx);

      Func<Task> act = async () => await repo.CreateAsync(new StockMovement { ProductId = 9999999, MovementQuantity = 1 });

      await act.Should().ThrowAsync<KeyNotFoundException>()
        .WithMessage("*not found*");
    }

    [Fact]
    public async Task ProductSearchByStock_ReturnsProductsWithinRange()
    {
      await using var ctx = CreateContext();
      // create products
      ctx.Products.Add(new Product { Name = "StockRange A" });
      ctx.Products.Add(new Product { Name = "StockRange B" });
      ctx.Products.Add(new Product { Name = "StockRange C" });
      await ctx.SaveChangesAsync();

      var allProducts = await ctx.Products.ToListAsync();
      var repo = new StockMovementRepository(ctx);

      // Set stocks: A=5, B=10, C=20
      await repo.CreateAsync(new StockMovement { ProductId = allProducts.Single(p => p.Name == "StockRange A").Id, MovementQuantity = 5 });
      await repo.CreateAsync(new StockMovement { ProductId = allProducts.Single(p => p.Name == "StockRange B").Id, MovementQuantity = 10 });
      await repo.CreateAsync(new StockMovement { ProductId = allProducts.Single(p => p.Name == "StockRange C").Id, MovementQuantity = 20 });

      var productRepo = new Products.Infrastructure.Repositories.ProductRepository(ctx);
      var results = (await productRepo.ProductSearchByStock(6, 20)).Select(p => p.Name).ToList();

      results.Should().Contain("StockRange B");
      results.Should().Contain("StockRange C");
      results.Should().NotContain("StockRange A");
    }

    [Fact]
    public async Task ProductSearchByStock_ExcludesProductsOutsideRange()
    {
      await using var ctx = CreateContext();
      // create products
      ctx.Products.Add(new Product { Name = "StockRange X" });
      ctx.Products.Add(new Product { Name = "StockRange Y" });
      await ctx.SaveChangesAsync();

      var allProducts = await ctx.Products.ToListAsync();
      var repo = new StockMovementRepository(ctx);

      // Set stocks: X=2, Y=8
      await repo.CreateAsync(new StockMovement { ProductId = allProducts.Single(p => p.Name == "StockRange X").Id, MovementQuantity = 2 });
      await repo.CreateAsync(new StockMovement { ProductId = allProducts.Single(p => p.Name == "StockRange Y").Id, MovementQuantity = 8 });

      var productRepo = new Products.Infrastructure.Repositories.ProductRepository(ctx);
      var results = (await productRepo.ProductSearchByStock(3, 5)).Select(p => p.Name).ToList();

      results.Should().BeEmpty();
    }

    [Fact]
    public async Task ProductSearchByStock_ReturnsEmpty_WhenNoProductsInRange()
    {
      await using var ctx = CreateContext();
      // create products
      ctx.Products.Add(new Product { Name = "StockRange Z" });
      await ctx.SaveChangesAsync();

      var product = await ctx.Products.SingleAsync(p => p.Name == "StockRange Z");
      var repo = new StockMovementRepository(ctx);

      // Set stock: Z=50
      await repo.CreateAsync(new StockMovement { ProductId = product.Id, MovementQuantity = 50 });

      var productRepo = new Products.Infrastructure.Repositories.ProductRepository(ctx);
      var results = await productRepo.ProductSearchByStock(0, 10);

      results.Should().BeEmpty();
    }
  }
}