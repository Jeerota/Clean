namespace Clean.Application.Common
{
    public interface IRepository<TContext, TEntity> : IReadOnlyRepository<TContext, TEntity> where TContext : class, new() where TEntity : class
    {
        void Create(TEntity entity);
        Task CreateAsync(TEntity entity);
        void Update(TEntity entity);
        Task UpdateAsync(TEntity entity);
        void Delete(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}