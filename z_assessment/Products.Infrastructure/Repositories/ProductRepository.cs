using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Products.Domain.Interfaces;
using Products.Domain.Entities;

namespace Products.Infrastructure.Repositories
{
  public class ProductRepository : BaseRepo<Product>, IProductRepository
  {
    protected readonly AppDBContext _context;
    public ProductRepository(AppDBContext context) : base(context)
    {
      _context= context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Product>> ProductSearch(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        return await _context.Products.ToListAsync();


      return await _context.Products
                           .Where(p => EF.Functions.Like(p.Name, $"%{name}%"))
                           .ToListAsync();
    }

    public async Task<IEnumerable<Product>> ProductSearchByStock(int min, int max)
    {
      if (min > max) (min, max) = (max, min);
      return await _context.Products
                           .Where(p => p.Stock >= min && p.Stock <= max)
                           .ToListAsync();
    }


  }
}
