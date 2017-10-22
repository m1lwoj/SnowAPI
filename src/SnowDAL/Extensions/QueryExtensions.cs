using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SnowDAL.Extensions
{
    public static class QueryExtensions
    {
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty, bool desc) where TEntity : class
        {
            string command = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExpression));
            return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(resultExpression);
        }

        public static IOrderedQueryable<TEntity> ThenBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty, bool desc) where TEntity : class
        {
            string command = desc ? "ThenByDescending" : "ThenBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExpression));
            return (IOrderedQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(resultExpression);
        }

        public static IQueryable<T> WhereGeneric<T>(this IQueryable<T> query, string propertyName, object propertyValue)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "type");
            var propertyExpression = Expression.Property(parameter, propertyName);

            var type = typeof(T).GetProperty(propertyName);
            Type propertyType = type.PropertyType;
            Expression expression, someValue;
            MethodInfo method;
            ApplyComparingMethod(propertyValue, propertyType, out expression, out someValue, out method);

            expression = Expression.Call(propertyExpression, method, someValue);
            return query.Where(Expression.Lambda<Func<T, bool>>(expression, parameter));
        }

        private static void ApplyComparingMethod(object propertyValue, Type propertyType, out Expression expression, out Expression expressionResult, out MethodInfo method)
        {
            expression = null;
            expressionResult = null;
            method = null;
            if (propertyType == typeof(string))
            {
                method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                expressionResult = Expression.Constant(propertyValue, typeof(string));
            }
            else if (propertyType == typeof(short) || propertyType == typeof(short?))
            {
                method = typeof(short).GetMethod("Equals", new[] { typeof(short) });
                expressionResult = Expression.Constant(propertyValue, typeof(short));
            }
            else if (propertyType == typeof(int) || propertyType == typeof(int?))
            {
                method = typeof(int).GetMethod("Equals", new[] { typeof(int) });
                expressionResult = Expression.Constant(propertyValue, typeof(int));
            }
            else if (propertyType == typeof(double) || propertyType == typeof(double?))
            {
                method = typeof(short).GetMethod("Equals", new[] { typeof(double) });
                expressionResult = Expression.Constant(propertyValue, typeof(double));
            }
            else if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
            {
                method = typeof(short).GetMethod("Equals", new[] { typeof(decimal) });
                expressionResult = Expression.Constant(propertyValue, typeof(decimal));
            }
        }
    }
}
