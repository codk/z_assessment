namespace Products.Domain

{
  public interface IBaseRepo<T> where T : class
  {
    public Task<IEnumerable<T>> GetAllAsync();
    public Task<int> CreateAsync(T entity);
    public Task<T> GetByIdAsync(int id);
    public Task<bool> UpdateAsync(T entity);
    public Task<bool> DeleteAsync(int id);
  }
}
