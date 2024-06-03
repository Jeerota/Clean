using Clean.Domain.Common.Models;

namespace Clean.Domain.Common.Interfaces
{
    public interface IRepository<TContext> : IReadOnlyRepository<TContext> where TContext : class
    {
        ResultResponse<TDto> Create<TDto, TEntity>(TDto dto) where TDto : class where TEntity : class, new();
        Task<ResultResponse<TDto>> CreateAsync<TDto, TEntity>(TDto dto) where TDto : class where TEntity : class, new();
        ResultResponse<TDto> Update<TDto, TEntity>(TDto dto) where TDto : class where TEntity : class, new();
        Task<ResultResponse<TDto>> UpdateAsync<TDto, TEntity>(TDto dto) where TDto : class where TEntity : class, new();
        ResultResponse<TDto> Delete<TDto, TEntity>(object?[]? primaryKey) where TDto : class where TEntity : class, new();
        Task<ResultResponse<TDto>> DeleteAsync<TDto, TEntity>(object?[]? primaryKey) where TDto : class where TEntity : class, new();
    }
}