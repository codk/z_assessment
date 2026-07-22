using Products.DTO;

namespace Products.interfaces
{
  public interface IBaseService<T, TResponseDto, TUpdateDto>
  {

    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<TResponseDto> CreateAsync(T dto);
    Task<TResponseDto?> UpdateAsync(int id, TUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    
  }
}
