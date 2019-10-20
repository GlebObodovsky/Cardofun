using System;
using System.Collections.Generic;

namespace Cardofun.Core.Enumerables
{
    public class PagedList<T>: List<T>
    {
        /// <summary>
        /// Current page
        /// </summary>
        /// <value></value>
        public Int32 PageNumber { get; set; }
        /// <summary>
        /// Size of each page
        /// </summary>
        /// <value></value>
        public Int32 PageSize { get; set; }
        /// <summary>
        /// Total amount of items in the repository
        /// </summary>
        /// <value></value>
        public Int32 TotalCount { get; set; }
        /// <summary>
        /// Total amount of pages in the repository
        /// </summary>
        /// <returns></returns>
        public Int32 TotalPages => (Int32)Math.Ceiling(TotalCount / (Decimal)PageSize);
        public PagedList(IEnumerable<T> items, Int32 count, Int32 pageNumber, Int32 pageSize)
        {
            this.AddRange(items);
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}