using Products.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Products.Infrastructure.Repositories
{
  public class BaseRepo<T> : IBaseRepo<T> where T : class
  {
    private readonly AppDBContext _context;

    public BaseRepo(AppDBContext context)
    {
      _context = context;
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
      return await _context.Set<T>().ToListAsync();
    }

    public async Task<int> CreateAsync(T entity)
    {
      await _context.AddAsync(entity);
      return await _context.SaveChangesAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
      // Uses DbContext.FindAsync which accepts key values; returns null if not found.
      return await _context.FindAsync<T>(id);
    }

    public async Task<bool> UpdateAsync(T entity)
    {
      _context.Update(entity);
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
      var entity = await _context.FindAsync<T>(id);
      if (entity == null) return false;

      _context.Remove(entity);
      return await _context.SaveChangesAsync() > 0;
    }
  }
}
