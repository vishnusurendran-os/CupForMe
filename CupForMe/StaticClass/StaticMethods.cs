using CupForMe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CupForMe.StaticClass
{
    public static class StaticMethods
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, SearchStateModel searchStateModels)
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            string command = "OrderBy";
            if (searchStateModels.Sort == "desc")
            {
                command = "OrderByDescending";
            }
            Expression resultExpression = null;
            var property = typeof(T).GetProperty(searchStateModels.SortField);
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { typeof(T), property.PropertyType }, query.Expression, Expression.Quote(orderByExpression));
            return query.Provider.CreateQuery<T>(resultExpression);
        }
    }
}
