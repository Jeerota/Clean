using Clean.Domain.Common.Enums;
using Clean.Domain.Common.Helpers;
using System.Linq.Expressions;
using System.Reflection;

namespace Clean.Domain.Common.Model
{
    public abstract class LookupRequest
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public FilterType FilterType { get; set; } = FilterType.Equal;

        public Expression<Func<TEntity, bool>> BuildPreciate<TEntity>() where TEntity : class
        {
            Expression<Func<TEntity, bool>>? predicate = null;
            IEnumerable<PropertyInfo>? properties = GetType().GetProperties()
                .Where(prop => prop.Name != "FilterType");

            if (properties.Any())
            {
                ParameterExpression item = Expression.Parameter(typeof(TEntity), "item");

                foreach (PropertyInfo property in properties)
                {
                    var lookupPropertyValue = property.GetValue(this, null);

                    if (lookupPropertyValue != null)
                    {
                        MemberExpression prop = Expression.Property(item, property.Name);
                        PropertyInfo? propertyInfo = typeof(TEntity).GetProperty(property.Name);

                        if (propertyInfo != null)
                        {
                            ConstantExpression value = Expression.Constant(Convert.ChangeType(lookupPropertyValue, propertyInfo.PropertyType));
                            Expression equal = Expression.Equal(prop, value);
                            switch (FilterType)
                            {
                                case FilterType.Equal:
                                    equal = Expression.Equal(prop, value);
                                    break;
                                case FilterType.NotEqual:
                                    equal = Expression.NotEqual(prop, value);
                                    break;
                                case FilterType.GreaterThan:
                                    equal = Expression.GreaterThan(prop, value);
                                    break;
                                case FilterType.LessThan:
                                    equal = Expression.LessThan(prop, value);
                                    break;
                                default:
                                    equal = Expression.Equal(prop, value);
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
