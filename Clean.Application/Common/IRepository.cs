using Microsoft.EntityFrameworkCore;

namespace Clean.Application.Common
{
    public interface IRepository<TContext, TEntity> : IReadOnlyRepository<TContext, TEntity> where TContext : DbContext, new() where TEntity : class
    {
        void Create(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}