using Clean.Domain.Common.Enums;
using Clean.Domain.Common.Helpers;
using System.Linq.Expressions;
using System.Reflection;

namespace Clean.Domain.Common.Models
{
    public abstract class LookupRequest
    {
        private HashSet<string> ExcludedPredicateProperties
        {
            get
            {
                return new HashSet<string>() { "FilterType", "Page", "PageLimit", "OrderBy", "OrderType" };
            }
        }

        public List<string>? OrderBy { get; set; }
        public string? OrderType { get; set; }
        public int? Page { get; set; }
        public int? PageLimit { get; set; }

        //ToDo: Add FilterType per column.
        public FilterType FilterType { get; set; } = FilterType.Equal;

        public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? GetOrderBy<TEntity>() where TEntity : class
        {
            if (OrderBy == null
                || !OrderBy.Any())
                return null;

            Type typeQueryable = typeof(IQueryable<TEntity>);
            ParameterExpression argQueryable = Expression.Parameter(typeQueryable, "p");
            var outerExpression = Expression.Lambda(argQueryable, argQueryable);
            Type type = typeof(TEntity);
            ParameterExpression arg = Expression.Parameter(type, "x");

            Expression expr = arg;
            foreach (string column in OrderBy)
            {
                PropertyInfo property = type.GetProperty(column, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                expr = Expression.Property(expr, property);
                type = property.PropertyType;
            }
            LambdaExpression lambda = Expression.Lambda(expr, arg);
            string methodName = OrderType.Equals("DESC", StringComparison.OrdinalIgnoreCase) ? "OrderBy" : "OrderByDescending";

            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(TEntity), type }, outerExpression.Body, Expression.Quote(lambda));
            var finalLambda = Expression.Lambda(resultExp, argQueryable);
            return (Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>)finalLambda.Compile();
        }

        public Expression<Func<TEntity, bool>> BuildPredicate<TEntity>() where TEntity : class
        {
            Expression<Func<TEntity, bool>>? predicate = null;
            IEnumerable<PropertyInfo>? properties = GetType().GetProperties()
                .Where(prop => !ExcludedPredicateProperties.Contains(prop.Name));

            if (properties.Any())
            {
                ParameterExpression item = Expression.Parameter(typeof(TEntity), "item");

                foreach (PropertyInfo property in properties)
                {
                    var lookupPropertyValue = property.GetValue(this, null);

                    if (lookupPropertyValue != null)
                    {
                        if (item.GetType().GetProperty(property.Name) == null)
                            continue;

                        MemberExpression prop = Expression.Property(item, property.Name);
                        PropertyInfo? propertyInfo = typeof(TEntity).GetProperty(property.Name);

                        if (propertyInfo != null)
                        {
                            ConstantExpression filter = Expression.Constant(
                                Convert.ChangeType(lookupPropertyValue, propertyInfo.PropertyType.GenericTypeArguments[0]));
                            Expression typeFilter = Expression.Convert(filter, propertyInfo.PropertyType);
                            Expression equal;
                            switch (FilterType)
                            {
                                case FilterType.Equal:
                                    equal = Expression.Equal(prop, typeFilter);
                                    break;
                                case FilterType.NotEqual:
                                    equal = Expression.NotEqual(prop, typeFilter);
                                    break;
                                case FilterType.GreaterThan:
                                    equal = Expression.GreaterThan(prop, typeFilter);
                                    break;
                                case FilterType.LessThan:
                                    equal = Expression.LessThan(prop, typeFilter);
                                    break;
                                default:
                                    equal = Expression.Equal(prop, typeFilter);
                                    break;

                            }
                            Expression<Func<TEntity, bool>> lambda = Expression.Lambda<Func<TEntity, bool>>(equal, item);

                            if (predicate == null)
                                predicate = PredicateBuilder.Create(lambda);
                            else
                                predicate = predicate.And(lambda);
                        }
                    }
                }
            }

            if (predicate == null)
                predicate = PredicateBuilder.True<TEntity>();

            return predicate;
        }
    }
}
