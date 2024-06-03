using Clean.Domain.Common.Models;
using System.Linq.Expressions;

namespace Clean.Domain.Common.Interfaces
{
    public interface IReadOnlyRepository<TContext> where TContext : class
    {
        TDto First<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include) where TEntity : class;
        Task<TDto> FirstAsync<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include) where TEntity : class;
        TDto? FirstOrDefault<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include) where TEntity : class;
        Task<TDto?> FirstOrDefaultAsync<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include) where TEntity : class;
        TDto Single<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include) where TEntity : class;
        Task<TDto> SingleAsync<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include) where TEntity : class;
        TDto? SingleOrDefault<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include) where TEntity : class;
        Task<TDto?> SingleOrDefaultAsync<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include) where TEntity : class;
        IQueryable<TEntity> GetQueryable<TEntity>(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, params Expression<Func<TEntity, object>>[] include) where TEntity : class;
        IEnumerable<TDto> GetEnumerable<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate, int? page = null, int? recordCount = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, params Expression<Func<TEntity, object>>[] include) where TEntity : class;
        FetchResponse<TDto> GetFetchResponse<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate, int? page, int? recordCount = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, params Expression<Func<TEntity, object>>[] include) where TDto : class where TEntity : class;
    }
}