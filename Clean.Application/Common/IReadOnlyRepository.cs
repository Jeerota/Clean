using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Clean.Application.Common
{
    public interface IReadOnlyRepository<TContext, TEntity> where TContext : DbContext, new() where TEntity : class
    {
        TEntity First(Expression<Func<TEntity, bool>> predicte, params Expression<Func<TEntity, object>>[] include);
        TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicte, params Expression<Func<TEntity, object>>[] include);
        TEntity Single(Expression<Func<TEntity, bool>> predicte, params Expression<Func<TEntity, object>>[] include);
        TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicte, params Expression<Func<TEntity, object>>[] include);
        IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicte, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, params Expression<Func<TEntity, object>>[] include);
        IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, params Expression<Func<TEntity, object>>[] include);
    }
}