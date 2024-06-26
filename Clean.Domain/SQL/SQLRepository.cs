using AutoMapper;
using Clean.Domain.Common.Interfaces;
using Clean.Domain.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using Clean.Infrastructure.SQL;

namespace Clean.Domain.SQL
{
    public class SQLRepository<TContext> : IRepository<TContext> where TContext : DbContext, new()
    {
        private readonly DbContext _dbContext;
        private const int _maxPagination = 500;
        private readonly IMapper _mapper;

        public SQLRepository(IDbContextFactory dbContextFactory)
        {
            _dbContext = dbContextFactory.GetDbContext<TContext>();
            MapperConfiguration mapperConfig = AutoMapperConfigurationHelper.GetAutoMapperConfiguration();
            _mapper = mapperConfig.CreateMapper();
        }

        private TEntity? Find<TEntity>(params object?[]? keyValues) where TEntity : class
        {
            return _dbContext.Find<TEntity>(keyValues);
        }

        private async Task<TEntity?> FindAsync<TEntity>(params object?[]? keyValues) where TEntity : class
        {
            return await _dbContext.FindAsync<TEntity>(keyValues);
        }

        public ResultResponse<TDto> Create<TDto, TEntity>(TDto dto) where TDto : class where TEntity : class, new()
        {
            ResultResponse<TDto> result = new();
            try
            {
                TEntity entity = _mapper.Map<TEntity>(dto);
                EntityEntry<TEntity> entry = _dbContext.Entry(entity);
                object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                    .Select(x => entry.Property(x.Name).CurrentValue)
                    .ToArray();

                if (Find<TEntity>(primaryKey) == null)
                {
                    PropertyEntry? createdDate = entry?.Properties.SingleOrDefault(x => x.Metadata.Name == "CreatedDatetime");

                    if (createdDate != null)
                        createdDate.CurrentValue = DateTime.UtcNow;

                    _dbContext.Add(entity);
                    _dbContext.SaveChanges();

                    result.Entity = _mapper.Map<TDto>(entity);
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

        public async Task<ResultResponse<TDto>> CreateAsync<TDto, TEntity>(TDto dto) where TDto : class where TEntity : class, new()
        {
            ResultResponse<TDto> result = new();
            try
            {
                TEntity entity = _mapper.Map<TEntity>(dto);
                EntityEntry<TEntity> entry = _dbContext.Entry(entity);
                object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                    .Select(x => entry.Property(x.Name).CurrentValue)
                    .ToArray();

                if (await FindAsync<TEntity>(primaryKey) == null)
                {
                    PropertyEntry? createdDate = entry?.Properties.SingleOrDefault(x => x.Metadata.Name == "CreatedDatetime");

                    if (createdDate != null)
                        createdDate.CurrentValue = DateTime.UtcNow;

                    await _dbContext.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();

                    result.Entity = _mapper.Map<TDto>(entity);
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

        public ResultResponse<TDto> Update<TDto, TEntity>(TDto dto) where TDto : class where TEntity : class, new()
        {
            ResultResponse<TDto> result = new();
            try
            {
                TEntity entity = _mapper.Map<TEntity>(dto);
                EntityEntry<TEntity> entry = _dbContext.Entry(entity);
                object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                    .Select(x => entry.Property(x.Name).CurrentValue)
                    .ToArray();

                if (Find<TEntity>(primaryKey) != null)
                {
                    PropertyEntry? modifiedDate = entry?.Properties.SingleOrDefault(x => x.Metadata.Name == "ModifiedDatetime");

                    if (modifiedDate != null)
                        modifiedDate.CurrentValue = DateTime.UtcNow;

                    _dbContext.Update(entity);
                    _dbContext.SaveChanges();

                    result.Entity = _mapper.Map<TDto>(entity);
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

        public async Task<ResultResponse<TDto>> UpdateAsync<TDto, TEntity>(TDto dto) where TDto : class where TEntity : class, new()
        {
            ResultResponse<TDto> result = new();
            try
            {
                TEntity entity = _mapper.Map<TEntity>(dto);
                EntityEntry<TEntity> entry = _dbContext.Entry(entity);
                object?[]? primaryKey = entry?.Metadata?.FindPrimaryKey()?.Properties
                    .Select(x => entry.Property(x.Name).CurrentValue)
                    .ToArray();

                if (await FindAsync<TEntity>(primaryKey) != null)
                {
                    PropertyEntry? modifiedDate = entry?.Properties.SingleOrDefault(x => x.Metadata.Name == "ModifiedDatetime");

                    if (modifiedDate != null)
                        modifiedDate.CurrentValue = DateTime.UtcNow;

                    _dbContext.Update(entity);
                    await _dbContext.SaveChangesAsync();

                    result.Entity = _mapper.Map<TDto>(entity);
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

        private List<object> ParsePrimaryKey(string?[]? primaryKeyTypes, object?[]? primaryKey)
        {
            List<object> typedPrimaryKey = new();
            for (int i = 0; i < primaryKeyTypes.Length; i++)
            {
                string stringValue = primaryKey[i]?.ToString();
                switch (primaryKeyTypes[i])
                {
                    case "Int32":
                        typedPrimaryKey.Add(int.TryParse(stringValue, out int intResult) ? intResult : 0);
                        break;
                    case "Int64":
                        typedPrimaryKey.Add(long.TryParse(stringValue, out long longResult) ? longResult : 0);
                        break;
                    case "Guid":
                        typedPrimaryKey.Add(Guid.TryParse(stringValue, out Guid guidResult) ? guidResult : Guid.Empty);
                        break;
                    case "DateTime":
                        typedPrimaryKey.Add(DateTime.TryParse(stringValue, out DateTime dateTimeResult) ? dateTimeResult : DateTime.MinValue);
                        break;
                    case "String":
                        typedPrimaryKey.Add(stringValue != null ? stringValue : string.Empty);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(primaryKeyTypes[i]);
                }
            }

            return typedPrimaryKey;
        }

        public ResultResponse<TDto> Delete<TDto, TEntity>(object?[]? primaryKey) where TDto : class where TEntity : class, new()
        {
            ResultResponse<TDto> result = new();

            if (primaryKey == null)
            {
                result.Errors.Add($"Invalid {nameof(primaryKey)}");
                return result;
            }

            EntityEntry<TEntity> entry = _dbContext.Entry(new TEntity());
            string?[]? primaryKeyTypes = entry?.Metadata?.FindPrimaryKey()?.Properties
                .Select(x => entry.Property(x.Name)?.Metadata?.FindTypeMapping()?.ClrType?.Name)
                .ToArray();
            List<object> typedPrimaryKey = ParsePrimaryKey(primaryKeyTypes, primaryKey);

            try
            {
                if (primaryKey.Count() != primaryKeyTypes.Count())
                    throw new ArgumentOutOfRangeException("Provided primary key does not match column length of entity.");

                TEntity? existingEntity = Find<TEntity>(typedPrimaryKey.ToArray());

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

        public async Task<ResultResponse<TDto>> DeleteAsync<TDto, TEntity>(object?[]? primaryKey) where TDto : class where TEntity : class, new()
        {
            TEntity entity = new TEntity();
            EntityEntry<TEntity> entry = _dbContext.Entry(entity);
            string?[]? primaryKeyTypes = entry?.Metadata?.FindPrimaryKey()?.Properties
                .Select(x => entry.Property(x.Name)?.Metadata.FindTypeMapping().ClrType.Name)
                .ToArray();
            List<object> typedPrimaryKey = ParsePrimaryKey(primaryKeyTypes, primaryKey);

            ResultResponse<TDto> result = new();
            try
            {
                if (primaryKey.Count() != primaryKeyTypes.Count())
                    throw new ArgumentOutOfRangeException("Provided primary key does not match column length of entity.");

                TEntity? existingEntity = await FindAsync<TEntity>(primaryKey);

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

        private IQueryable<TEntity> IncludeInQuery<TEntity>(Expression<Func<TEntity, object>>[] include) where TEntity : class
        {
            return include.Aggregate(_dbContext.Set<TEntity>().AsQueryable(), (current, item) => EvaluateInclude(current, item));
        }

        private IQueryable<TEntity> EvaluateInclude<TEntity>(IQueryable<TEntity> current, Expression<Func<TEntity, object>> item) where TEntity : class
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

        public TDto First<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include) where TEntity : class
        {
            TEntity entity = include.Any()
                ? IncludeInQuery(include).First(predicate)
                : _dbContext.Set<TEntity>().First(predicate);

            return _mapper.Map<TDto>(entity);
        }

        public async Task<TDto> FirstAsync<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include) where TEntity : class
        {
            TEntity entity = include.Any()
                ? await IncludeInQuery(include).FirstAsync(predicate)
                : await _dbContext.Set<TEntity>().FirstAsync(predicate);

            return _mapper.Map<TDto>(entity);
        }

        public TDto? FirstOrDefault<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include) where TEntity : class
        {
            TEntity? entity = include.Any()
                ? IncludeInQuery(include).FirstOrDefault(predicate)
                : _dbContext.Set<TEntity>().FirstOrDefault(predicate);

            return _mapper.Map<TDto>(entity);
        }

        public async Task<TDto?> FirstOrDefaultAsync<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include) where TEntity : class
        {
            TEntity? entity = include.Any()
                ? await IncludeInQuery(include).FirstOrDefaultAsync(predicate)
                : await _dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);

            return _mapper.Map<TDto>(entity);
        }

        public TDto Single<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include) where TEntity : class
        {
            TEntity entity = include.Any()
                ? IncludeInQuery(include).Single(predicate)
                : _dbContext.Set<TEntity>().Single(predicate);

            return _mapper.Map<TDto>(entity);
        }

        public async Task<TDto> SingleAsync<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include) where TEntity : class
        {
            TEntity entity = include.Any()
                ? await IncludeInQuery(include).SingleAsync(predicate)
                : await _dbContext.Set<TEntity>().SingleAsync(predicate);

            return _mapper.Map<TDto>(entity);
        }

        public TDto? SingleOrDefault<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include) where TEntity : class
        {
            TEntity? entity = include.Any()
                ? IncludeInQuery(include).SingleOrDefault(predicate)
                : _dbContext.Set<TEntity>().SingleOrDefault(predicate);

            return _mapper.Map<TDto>(entity);
        }

        public async Task<TDto?> SingleOrDefaultAsync<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] include) where TEntity : class
        {
            TEntity? entity = include.Any()
                ? await IncludeInQuery(include).SingleOrDefaultAsync(predicate)
                : await _dbContext.Set<TEntity>().SingleOrDefaultAsync(predicate);

            return _mapper.Map<TDto>(entity);
        }

        public IQueryable<TEntity> GetQueryable<TEntity>(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] include) where TEntity : class
        {
            IQueryable<TEntity> query = include.Any()
                ? IncludeInQuery(include).AsNoTracking().Where(predicate)
                : _dbContext.Set<TEntity>().AsNoTracking().Where(predicate);

            return orderBy != null
                ? orderBy(query)
                : query;
        }

        public IEnumerable<TDto> GetEnumerable<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate,
            int? page = null,
            int? requestedLimit = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] include) where TEntity : class
        {
            IQueryable<TEntity> query = GetQueryable(predicate, orderBy, include);
            int pageLimit = requestedLimit ?? 0;
            int skipCount = ((page ?? 1) - 1) * pageLimit;

            IEnumerable<TEntity> entities = pageLimit > 0
                ? query.Skip(skipCount).Take(pageLimit).AsEnumerable()
                : query.AsEnumerable();

            return entities.Select(entity => _mapper.Map<TDto>(entity));
        }

        public FetchResponse<TDto> GetFetchResponse<TDto, TEntity>(Expression<Func<TEntity, bool>> predicate,
            int? page,
            int? requestedLimit = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            params Expression<Func<TEntity, object>>[] include) where TDto : class where TEntity : class
        {
            FetchResponse<TDto> response = new();

            IQueryable<TEntity> query = GetQueryable(predicate, orderBy, include);
            int pageLimit = Math.Min(requestedLimit ?? _maxPagination, _maxPagination);
            int skipCount = ((page ?? 1) - 1) * pageLimit;

            response.TotalRecords = query.Count();
            response.Page = page ?? 1;
            response.PageLimit = pageLimit;

            IEnumerable<TEntity> entities = query.Skip(skipCount).Take(pageLimit);
            response.Records = entities.Select(entity => _mapper.Map<TDto>(entity));

            return response;
        }
    }
}