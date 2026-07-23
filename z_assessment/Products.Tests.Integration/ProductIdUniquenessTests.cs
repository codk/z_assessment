using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;
using Products.Infrastructure;
using Testcontainers.PostgreSql;

namespace Products.Tests.Integration
{
  //https://testcontainers.com/guides/getting-started-with-testcontainers-for-dotnet/
  public class ProductIdUniquenessTests : IAsyncLifetime 
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
    public async Task ConcurrentInserts_AllIdsAreUnique()
    {
      const int count = 30;

      // DBContext for each task for extra isolation
      IEnumerable<int> tcount = new List<int>(count);

      var tasks = tcount.Select(i => Task.Run(async () =>
        {
          await using var ctx = CreateContext();
          var product = new Product { Name = $"Concurrent {i}", Stock = i };
          ctx.Products.Add(product);
          await ctx.SaveChangesAsync();
          return product.Id;
        }));

      var ids = await Task.WhenAll(tasks);

      ids.Should().OnlyHaveUniqueItems("never repeats");
      ids.Should().AllSatisfy(id =>
          id.Should().BeInRange(100000, 999999, "Range ok"));
    }

    [Fact]
    public async Task SequentialInserts_IdsAreStrictlyIncreasing()
    {
      var ids = new List<int>();
      for (int i = 0; i < 5; i++)
      {
        await using var ctx = CreateContext();
        var product = new Product { Name = $"Sequential {i}", Stock = 0 };
        ctx.Products.Add(product);
        await ctx.SaveChangesAsync();
        ids.Add(product.Id);
      }

      ids.Should().BeInAscendingOrder("sequence is raising");
    }
  }
}