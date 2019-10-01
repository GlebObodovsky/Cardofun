using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Cardofun.DataContext.Helpers
{
    public static class ExtentionMethods
    {
        public static IQueryable<T> IncludeAll<T>(this IQueryable<T> queryable, params Expression<Func<T, object>>[] includes)
            where T: class
        {
			foreach (var query in includes)
                queryable = queryable.Include(query);
            
            return queryable;
        }
    }
}