using Clean.Domain.Common.Models;

namespace Clean.Domain.Common.Interfaces
{
    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
    {
        ResultResponse<TEntity> Create(TEntity entity);
        Task<ResultResponse<TEntity>> CreateAsync(TEntity entity);
        ResultResponse<TEntity> Update(TEntity entity);
        Task<ResultResponse<TEntity>> UpdateAsync(TEntity entity);
        ResultResponse<TEntity> Delete(object?[]? primaryKey);
        Task<ResultResponse<TEntity>> DeleteAsync(object?[]? primaryKey);
    }
}