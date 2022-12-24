using Clean.Domain.Common.Interfaces;
using Clean.Domain.Common.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Clean.Infrastructure.SQL
{
    public class SQLRepository<TContext, TEntity> : IRepository<TEntity> where TContext : DbContext, new() where TEntity : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _entity;

        public SQLRepository(IDbContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.GetDbContext<TContext>();
            _entity = _dbContext.Set<TEntity>();
        }

        private TEntity? Find(params object?[]? keyValues)
        {
            return _dbContext.Find<TEntity>(keyValues);
        }

        private async Task<TEntity?> FindAsync(params object?[]? keyValues)
        {
            return await _dbContext.FindAsync<TEntity>(keyValues);
        }

        public ResultResponse Create(TEntity entity)
        {
            ResultResponse result = new();
            try
            {
                EntityEntry<TEntity> entry = _dbContext.Entry(entity);
                object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                    .Select(x => entry.Property(x.Name).CurrentValue)
                    .ToArray();

                if (Find(primaryKey) == null)
                {
                    PropertyEntry? createdDate = entry?.Properties.SingleOrDefault(x => x.Metadata.Name == "CreatedDate");

                    if (createdDate != null)
                        createdDate.CurrentValue = DateTime.UtcNow;

                    _dbContext.Add(entity);
                    _dbContext.SaveChanges();
                }
                else
                    throw new InvalidOperationException($"Entity already exists for {typeof(TEntity)}");
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public async Task<ResultResponse> CreateAsync(TEntity entity)
        {
            ResultResponse result = new();
            try
            {
                EntityEntry<TEntity> entry = _dbContext.Entry(entity);
                object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                    .Select(x => entry.Property(x.Name).CurrentValue)
                    .ToArray();

                if (await FindAsync(primaryKey) == null)
                {
                    PropertyEntry? createdDate = entry?.Properties.SingleOrDefault(x => x.Metadata.Name == "CreatedDate");

                    if (createdDate != null)
                        createdDate.CurrentValue = DateTime.UtcNow;

                    _dbContext.Add(entity);
                    _dbContext.SaveChanges();
                }
                else
                    throw new InvalidOperationException($"Entity already exists for {typeof(TEntity)}");
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public ResultResponse Update(TEntity entity)
        {
            ResultResponse result = new();
            try
            {
                EntityEntry<TEntity> entry = _dbContext.Entry(entity);
                object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                    .Select(x => entry.Property(x.Name).CurrentValue)
                    .ToArray();

                TEntity? existingEntity = Find(primaryKey);

                if (existingEntity != null)
                {
                    PropertyEntry? modifiedDate = entry?.Properties.SingleOrDefault(x => x.Metadata.Name == "ModifiedDate");

                    if (modifiedDate != null)
                        modifiedDate.CurrentValue = DateTime.UtcNow;

                    _dbContext.Update(entity);
                    _dbContext.SaveChanges();
                }
                else
                    throw new InvalidOperationException($"Entity does not exist for {typeof(TEntity)}");
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public async Task<ResultResponse> UpdateAsync(TEntity entity)
        {
            ResultResponse result = new();
            try
            {
                EntityEntry<TEntity> entry = _dbContext.Entry(entity);
                object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                    .Select(x => entry.Property(x.Name).CurrentValue)
                    .ToArray();

                TEntity? existingEntity = await FindAsync(primaryKey);

                if (existingEntity != null)
                {
                    PropertyEntry? modifiedDate = entry?.Properties.SingleOrDefault(x => x.Metadata.Name == "ModifiedDate");

                    if (modifiedDate != null)
                        modifiedDate.CurrentValue = DateTime.UtcNow;

                    _dbContext.Update(entity);
                    _dbContext.SaveChanges();
                }
                else
                    throw new InvalidOperationException($"Entity does not exist for {typeof(TEntity)}");
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public ResultResponse Delete(TEntity entity)
        {
            ResultResponse result = new();
            try
            {
                EntityEntry<TEntity> entry = _dbContext.Entry(entity);
                object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                    .Select(x => entry.Property(x.Name).CurrentValue)
                    .ToArray();

                TEntity? existingEntity = Find(primaryKey);

                if (existingEntity != null)
                {
                    _dbContext.Remove(entity);
                    _dbContext.SaveChanges();
                }
                else
                    throw new InvalidOperationException($"Entity does not exist for {typeof(TEntity)}");
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public async Task<ResultResponse> DeleteAsync(TEntity entity)
        {
            ResultResponse result = new();
            try
            {
                EntityEntry<TEntity> entry = _dbContext.Entry(entity);
                object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                    .Select(x => entry.Property(x.Name).CurrentValue)
                    .ToArray();

                TEntity? existingEntity = await FindAsync(primaryKey);

                if (existingEntity != null)
                {
                    _dbContext.Remove(entity);
                    _dbContext.SaveChanges();
                }
                else
                    throw new InvalidOperationException($"Entity does not exist for {typeof(TEntity)}");
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        private IQueryable<TEntity> IncludeInQuery(Expression<Func<TEntity, object>>[] include)
        {
            IQueryable<TEntity> query = _entity.AsQueryable();
            return include.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public TEntity First(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include)
        {
            return include.Any()
                ? IncludeInQuery(include).First(predicate)
                : _entity.First(predicate);
        }

        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include)
        {
            return include.Any()
                ? await IncludeInQuery(include).FirstAsync(predicate)
                : await _entity.FirstAsync(predicate);
        }

        public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include)
        {
            return include.Any()
                ? IncludeInQuery(include).FirstOrDefault(predicate)
                : _entity.FirstOrDefault(predicate);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include)
        {
            return include.Any()
                ? await IncludeInQuery(include).FirstOrDefaultAsync(predicate)
                : await _entity.FirstOrDefaultAsync(predicate);
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include)
        {
            return include.Any()
                ? IncludeInQuery(include).Single(predicate)
                : _entity.Single(predicate);
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include)
        {
            return include.Any()
                ? await IncludeInQuery(include).SingleAsync(predicate)
                : await _entity.SingleAsync(predicate);
        }

        public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include)
        {
            return include.Any()
                ? IncludeInQuery(include).SingleOrDefault(predicate)
                : _entity.SingleOrDefault(predicate);
        }

        public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include)
        {
            return include.Any()
                ? await IncludeInQuery(include).SingleOrDefaultAsync(predicate)
                : await _entity.SingleOrDefaultAsync(predicate);
        }

        public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] include)
        {
            IQueryable<TEntity> query = include.Any()
                ? IncludeInQuery(include).Where(predicate)
                : _entity.Where(predicate);

            return orderBy != null
                ? orderBy(query).AsEnumerable()
                : query.AsEnumerable();
        }

        public IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] include)
        {
            IQueryable<TEntity> query = include.Any()
                ? IncludeInQuery(include).AsQueryable()
                : _entity.AsQueryable();

            return orderBy != null
                ? orderBy(query).AsEnumerable()
                : query.AsEnumerable();
        }
    }
}