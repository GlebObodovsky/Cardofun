using System;
using System.Linq;
using System.Threading.Tasks;
using Cardofun.Core.Enumerables;
using Microsoft.EntityFrameworkCore;

namespace Cardofun.DataContext.Helpers
{
    public static class ExtentionMethods
    {
        /// <summary>
        /// Converts IQueryable into PagedList
        /// </summary>
        /// <param name="pageNumber">Number of the needed page</param>
        /// <param name="pageSize">How many items should each page contains</param>
        /// <typeparam name="T">Type of items the list contains</typeparam>
        /// <returns></returns>
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> items, Int32 pageNumber, Int32 pageSize)
        {
            var count = await items.CountAsync();
            var itemsFromContext = await items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArrayAsync();
            return new PagedList<T>(itemsFromContext, count, pageNumber, pageSize);
        }
    }
}