using Clean.Application.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq.Expressions;

namespace Clean.Infrastructure.SQL
{
    public class SQLRepository<TContext, TEntity> : IRepository<TContext, TEntity> where TContext : DbContext, new() where TEntity : class
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

        public void Create(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Entry(entity);
            object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                .Select(x => entry.Property(x.Name).CurrentValue)
                .ToArray();

            if(Find(primaryKey) == null)
            {
                PropertyEntry? createdDate = entry?.Properties.SingleOrDefault(x => x.Metadata.Name == "CreatedDate");

                if(createdDate != null)
                    createdDate.CurrentValue = DateTime.UtcNow;

                _dbContext.Add(entity);
                _dbContext.SaveChanges();
            }
            else
                throw new InvalidOperationException($"Entity already exists for {typeof(TEntity)}");
        }

        public void Update(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Entry(entity);
            object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                .Select(x => entry.Property(x.Name).CurrentValue)
                .ToArray();

            TEntity? existingEntity = Find(primaryKey);

            if (existingEntity != null)
            {
                _dbContext.Update(entity);
                _dbContext.SaveChanges();
            }
            else
                throw new InvalidOperationException($"Entity does not exist for {typeof(TEntity)}");
        }

        public void Delete(TEntity entity)
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

        private IQueryable<TEntity> IncludeInQuery(Expression<Func<TEntity, object>>[] include)
        {
            IQueryable<TEntity> query = _entity.AsQueryable();
            return include.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public TEntity First(Expression<Func<TEntity, bool>> predicte,
            params Expression<Func<TEntity, object>>[] include)
        {
            return include != null
                ? IncludeInQuery(include).First(predicte)
                : _entity.First(predicte);
        }

        public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicte,
            params Expression<Func<TEntity, object>>[] include)
        {
            return include != null
                ? IncludeInQuery(include).FirstOrDefault(predicte)
                : _entity.FirstOrDefault(predicte);
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicte,
            params Expression<Func<TEntity, object>>[] include)
        {
            return include != null
                ? IncludeInQuery(include).Single(predicte)
                : _entity.Single(predicte);
        }

        public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicte,
            params Expression<Func<TEntity, object>>[] include)
        {
            return include != null
                ? IncludeInQuery(include).SingleOrDefault(predicte)
                : _entity.SingleOrDefault(predicte);
        }

        public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicte,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] include)
        {
            IQueryable<TEntity> query = include != null
                ? IncludeInQuery(include).Where(predicte)
                : _entity.Where(predicte);

            return orderBy != null
                ? orderBy(query).AsEnumerable()
                : query.AsEnumerable();
        }

        public IEnumerable<TEntity> GetAll(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
            params Expression<Func<TEntity, object>>[] include)
        {
            IQueryable<TEntity> query = include != null
                ? IncludeInQuery(include).AsQueryable()
                : _entity.AsQueryable();

            return orderBy != null
                ? orderBy(query).AsEnumerable()
                : query.AsEnumerable();
        }
    }
}