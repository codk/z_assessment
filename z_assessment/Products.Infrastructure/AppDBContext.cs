using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;
namespace Products.Infrastructure
{
  public class AppDBContext(DbContextOptions<AppDBContext> options) : DbContext(options)
  {
    public DbSet<Product> Products { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.HasSequence<int>("ProductIdSeq")
        .StartsAt(100000)
        .HasMin(100000)
        .HasMax(999999)
        .IsCyclic(false);

      modelBuilder.Entity<Product>(x=>{
        x.HasKey(p => p.Id);
        x.Property(p => p.Name).IsRequired().HasMaxLength(200);
        x.Property(p => p.Description).HasMaxLength(500);
        x.Property(p => p.Stock).IsRequired();
        x.Property(p => p.CreatedOn).HasDefaultValueSql("now()");
        x.Property(p => p.UpdatedOn).HasDefaultValueSql("now()").ValueGeneratedOnAddOrUpdate();

        x.Property(p => p.Id)
          .HasDefaultValueSql("nextval('\"ProductIdSeq\"')")
          .ValueGeneratedOnAdd();
      });

      modelBuilder.Entity<StockMovement>(x =>
      {
        x.HasKey(p => p.Id);
        x.Property(p => p.StockQuantity).IsRequired();
        x.Property(p => p.ProductId).IsRequired();
        x.Property(p => p.CreatedOn).HasDefaultValueSql("now()");
        x.HasIndex(p => p.ProductId);
      });
    }

  }

   
}
