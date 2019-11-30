using System;

namespace Cardofun.Core.ApiParameters
{
    /// <summary>
    /// Api parameters that needed for pagination
    /// </summary>
    public class PaginationParams
    {
        private const Int32 maxPageSize = 50;
        private Int32 pageSize = 10;

        /// <summary>
        /// Current user
        /// </summary>
        /// <value></value>
        public Int32 UserId { get; set; }
        /// <summary>
        /// Requested page number
        /// </summary>
        /// <value></value>
        public Int32 PageNumber { get; set; } = 1;
        /// <summary>
        /// Requested page size
        /// </summary>
        /// <value></value>
        public Int32 PageSize
        {
            get => pageSize;
            set => pageSize = value > maxPageSize ? maxPageSize : value;
        }
    }
}