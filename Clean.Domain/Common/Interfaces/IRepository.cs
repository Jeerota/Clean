using Clean.Domain.Common.Model;

namespace Clean.Domain.Common.Interfaces
{
    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
    {
        ResultResponse Create(TEntity entity);
        Task<ResultResponse> CreateAsync(TEntity entity);
        ResultResponse Update(TEntity entity);
        Task<ResultResponse> UpdateAsync(TEntity entity);
        ResultResponse Delete(TEntity entity);
        Task<ResultResponse> DeleteAsync(TEntity entity);
    }
}