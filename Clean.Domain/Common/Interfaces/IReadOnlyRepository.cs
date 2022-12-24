using System.Linq.Expressions;

namespace Clean.Domain.Common.Interfaces
{
    public interface IReadOnlyRepository<TEntity> where TEntity : class
    {
        TEntity First(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include);
        Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include);
        TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include);
        TEntity Single(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include);
        TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include);
        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] include);
        IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, params Expression<Func<TEntity, object>>[] include);
        IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, params Expression<Func<TEntity, object>>[] include);
    }
}