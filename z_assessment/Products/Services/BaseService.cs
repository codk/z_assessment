using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Products.interfaces;

namespace Products.Services
{
  public class BaseService<T, TResponseDto, TUpdateDto> : IBaseService<T, TResponseDto, TUpdateDto>
  {
      public virtual Task<IEnumerable<T>> GetAllAsync()
      {
          throw new NotImplementedException();
      }

      public virtual Task<T> GetByIdAsync(int id)
      {
          throw new NotImplementedException();
      }

      public virtual Task<TResponseDto> CreateAsync(T dto)
      {
          throw new NotImplementedException();
      }

      public virtual Task<TResponseDto?> UpdateAsync(int id, TUpdateDto dto)
      {
          throw new NotImplementedException();
      }

      public virtual Task<bool> DeleteAsync(int id)
      {
          throw new NotImplementedException();
      }
  }
}
