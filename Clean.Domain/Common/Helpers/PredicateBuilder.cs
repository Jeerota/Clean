using System.Linq.Expressions;

namespace Clean.Domain.Common.Helpers
{
    public static class PredicateBuilder
    {
        public static Expression<Func<TEntity, bool>> True<TEntity>() { return param => true; }
        public static Expression<Func<TEntity, bool>> False<TEntity>() { return param => false; }
        public static Expression<Func<TEntity, bool>> Create<TEntity>(Expression<Func<TEntity, bool>> predicate) { return predicate; }

        public static Expression<Func<TEntity, bool>> And<TEntity>(this Expression<Func<TEntity, bool>> first, Expression<Func<TEntity, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        public static Expression<Func<TEntity, bool>> Or<TEntity>(this Expression<Func<TEntity, bool>> first, Expression<Func<TEntity, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        public static Expression<Func<TEntity, bool>> Not<TEntity>(this Expression<Func<TEntity, bool>> expression)
        {
            var neagte = Expression.Negate(expression.Body);
            return Expression.Lambda<Func<TEntity, bool>>(neagte, expression.Parameters);
        }

        private static Expression<TEntity> Compose<TEntity>(this Expression<TEntity> first, Expression<TEntity> second, Func<Expression, Expression, Expression> merge)
        {
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.f, p => p.s);

            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            return Expression.Lambda<TEntity>(merge(first.Body, secondBody), first.Parameters);
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            readonly Dictionary<ParameterExpression, ParameterExpression> Map;

            ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                Map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression expression)
            {
                return new ParameterRebinder(map).Visit(expression);
            }

            protected override Expression VisitParameter(ParameterExpression parameterExpression)
            {
                if (Map.TryGetValue(parameterExpression, out ParameterExpression? replacement))
                {
                    parameterExpression = replacement;
                }

                return base.VisitParameter(parameterExpression);
            }
        }
    }
}