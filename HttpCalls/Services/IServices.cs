using HttpCalls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpCalls.Services
{
    public interface IServices<TEntity, in TPk> where TEntity : class
    {
        Task<ResponseObject<TEntity>> GetAsync();
        Task<ResponseObject<TEntity>> GetAsync(TPk id);
        Task<ResponseObject<TEntity>> CreateAsync(TEntity entity);
        Task<ResponseObject<TEntity>> UpdateAsync(TPk id,TEntity entity);
        Task<ResponseObject<TEntity>> DeleteAsync(TPk id);
    }
}
