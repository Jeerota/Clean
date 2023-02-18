using Clean.Domain.Common.Interfaces;
using Clean.Domain.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Clean.Infrastructure.SQL
{
    public class SQLRepository<TContext, TEntity> : IRepository<TEntity> where TContext : DbContext, new() where TEntity : class, new()
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _entity;
        private const int _maxPagination = 500;

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

        public ResultResponse<TEntity> Create(TEntity entity)
        {
            ResultResponse<TEntity> result = new();
            try
            {
                EntityEntry<TEntity> entry = _dbContext.Entry(entity);
                object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                    .Select(x => entry.Property(x.Name).CurrentValue)
                    .ToArray();

                if (Find(primaryKey) == null)
                {
                    PropertyEntry? createdDate = entry?.Properties.SingleOrDefault(x => x.Metadata.Name == "CreatedDatetime");

                    if (createdDate != null)
                        createdDate.CurrentValue = DateTime.UtcNow;

                    _dbContext.Add(entity);
                    _dbContext.SaveChanges();

                    result.Entity = entity;
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

        public async Task<ResultResponse<TEntity>> CreateAsync(TEntity entity)
        {
            ResultResponse<TEntity> result = new();
            try
            {
                EntityEntry<TEntity> entry = _dbContext.Entry(entity);
                object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                    .Select(x => entry.Property(x.Name).CurrentValue)
                    .ToArray();

                if (await FindAsync(primaryKey) == null)
                {
                    PropertyEntry? createdDate = entry?.Properties.SingleOrDefault(x => x.Metadata.Name == "CreatedDatetime");

                    if (createdDate != null)
                        createdDate.CurrentValue = DateTime.UtcNow;

                    await _dbContext.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();

                    result.Entity = entity;
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

        public ResultResponse<TEntity> Update(TEntity entity)
        {
            ResultResponse<TEntity> result = new();
            try
            {
                EntityEntry<TEntity> entry = _dbContext.Entry(entity);
                object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                    .Select(x => entry.Property(x.Name).CurrentValue)
                    .ToArray();

                TEntity? existingEntity = Find(primaryKey);

                if (existingEntity != null)
                {
                    if (existingEntity.Equals(entity))
                    {
                        result.Errors.Add("No changes detected.");
                        return result;
                    }

                    EntityEntry<TEntity> existingEntry = _dbContext.Entry(existingEntity);
                    PropertyEntry? modifiedDate = existingEntry?.Properties.SingleOrDefault(x => x.Metadata.Name == "ModifiedDatetime");

                    if (modifiedDate != null)
                        modifiedDate.CurrentValue = DateTime.UtcNow;

                    _dbContext.Update(existingEntity);
                    _dbContext.SaveChanges();

                    result.Entity = existingEntity;
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

        public async Task<ResultResponse<TEntity>> UpdateAsync(TEntity entity)
        {
            ResultResponse<TEntity> result = new();
            try
            {
                EntityEntry<TEntity> entry = _dbContext.Entry(entity);
                object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                    .Select(x => entry.Property(x.Name).CurrentValue)
                    .ToArray();

                TEntity? existingEntity = await FindAsync(primaryKey);

                if (existingEntity != null)
                {
                    if (existingEntity.Equals(entity))
                    {
                        result.Errors.Add("No changes detected.");
                        return result;
                    }

                    EntityEntry<TEntity> existingEntry = _dbContext.Entry(existingEntity);
                    PropertyEntry? modifiedDate = existingEntry?.Properties.SingleOrDefault(x => x.Metadata.Name == "ModifiedDatetime");

                    if (modifiedDate != null)
                        modifiedDate.CurrentValue = DateTime.UtcNow;

                    _dbContext.Update(existingEntity);
                    await _dbContext.SaveChangesAsync();

                    result.Entity = existingEntity;
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

        public ResultResponse<TEntity> Delete(object?[]? primaryKey)
        {
            ResultResponse<TEntity> result = new();

            if (primaryKey == null)
            {
                result.Errors.Add($"Invalid {nameof(primaryKey)}");
                return result;
            }    

            TEntity entity = new TEntity();
            EntityEntry<TEntity> entry = _dbContext.Entry(entity);
            string?[]? primaryKeyTypes = entry?.Metadata?.FindPrimaryKey()?.Properties
                .Select(x => entry.Property(x.Name)?.Metadata?.FindTypeMapping()?.ClrType?.Name)
                .ToArray();

            List<object> typedPrimaryKey = new();
            for(int i = 0; i < primaryKeyTypes.Length; i++)
            {
                string stringValue = primaryKey[i].ToString();
                typedPrimaryKey.Add(primaryKeyTypes[i] == "Int64"
                    ? long.TryParse(stringValue, out long longResult) ? longResult : 0
                    : primaryKeyTypes[i] == "Guid"
                        ? Guid.TryParse(stringValue, out Guid guidResult) ? guidResult : Guid.Empty
                        : DateTime.TryParse(stringValue, out DateTime dateTimeResult) ? dateTimeResult : DateTime.MinValue);
            }

            try
            {
                if (primaryKey.Count() != primaryKeyTypes.Count())
                    throw new ArgumentOutOfRangeException("Provided primary key does not match column length of entity.");

                TEntity? existingEntity = Find(typedPrimaryKey.ToArray());

                if (existingEntity != null)
                {
                    _dbContext.Remove(existingEntity);
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

        public async Task<ResultResponse<TEntity>> DeleteAsync(object?[]? primaryKey)
        {
            TEntity entity = new TEntity();
            EntityEntry<TEntity> entry = _dbContext.Entry(entity);
            string?[]? primaryKeyTypes = entry?.Metadata?.FindPrimaryKey()?.Properties
                .Select(x => entry.Property(x.Name)?.Metadata.FindTypeMapping().ClrType.Name)
                .ToArray();

            List<object> typedPrimaryKey = new List<object>();
            for (int i = 0; i < primaryKeyTypes.Length; i++)
            {
                string stringValue = primaryKey[i].ToString();
                typedPrimaryKey.Add(primaryKeyTypes[i] == "Int64"
                    ? long.TryParse(stringValue, out long longResult) ? longResult : 0
                    : primaryKeyTypes[i] == "Guid"
                        ? Guid.TryParse(stringValue, out Guid guidResult) ? guidResult : Guid.Empty
                        : DateTime.TryParse(stringValue, out DateTime dateTimeResult) ? dateTimeResult : DateTime.MinValue);
            }

            ResultResponse<TEntity> result = new();
            try
            {
                if (primaryKey.Count() != primaryKeyTypes.Count())
                    throw new ArgumentOutOfRangeException("Provided primary key does not match column length of entity.");

                TEntity? existingEntity = await FindAsync(primaryKey);

                if (existingEntity != null)
                {
                    _dbContext.Remove(existingEntity);
                    await _dbContext.SaveChangesAsync();
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
            return include.Aggregate(_entity.AsQueryable(), (current, item) => EvaluateInclude(current, item));
        }

        private IQueryable<TEntity> EvaluateInclude(IQueryable<TEntity> current, Expression<Func<TEntity, object>> item)
        {
            if (item.Body is MethodCallExpression)
            {
                var arguments = ((MethodCallExpression)item.Body).Arguments;
                if (arguments.Count > 1)
                {
                    var navigationPath = string.Empty;
                    for (var i = 0; i < arguments.Count; i++)
                    {
                        var arg = arguments[i];
                        var path = arg.ToString().Substring(arg.ToString().IndexOf('.') + 1);

                        navigationPath += (i > 0 ? "." : string.Empty) + path;
                    }
                    return current.Include(navigationPath);
                }
            }

            return current.Include(item);
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

        public IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] include)
        {
            IQueryable<TEntity> query = include.Any()
                ? IncludeInQuery(include).AsNoTracking().Where(predicate)
                : _entity.AsNoTracking().Where(predicate);

            return orderBy != null
                ? orderBy(query)
                : query;
        }

        public IEnumerable<TEntity> GetEnumerable(Expression<Func<TEntity, bool>> predicate, 
            int? page = null,
            int? requestedLimit = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] include)
        {
            IQueryable<TEntity> query = GetQueryable(predicate, orderBy, include);
            int pageLimit = requestedLimit ?? 0;
            int skipCount = ((page ?? 1) - 1) * pageLimit;

            return pageLimit > 0
                ? query.Skip(skipCount).Take(pageLimit).AsEnumerable()
                : query.AsEnumerable();
        }

        public FetchResponse<TEntity> GetFetchResponse(Expression<Func<TEntity, bool>> predicate,
            int? page,
            int? requestedLimit = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] include)
        {
            FetchResponse<TEntity> response = new();

            IQueryable<TEntity> query = GetQueryable(predicate, orderBy, include);
            int pageLimit = Math.Min(requestedLimit ?? _maxPagination, _maxPagination);
            int skipCount = ((page ?? 1) - 1) * pageLimit;

            response.TotalRecords = query.Count();
            response.Page = (page ?? 1);
            response.PageLimit = pageLimit;
            response.Entities = query.Skip(skipCount).Take(pageLimit);

            return response;
        }
    }
}