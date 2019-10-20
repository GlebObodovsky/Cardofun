using System;

namespace Cardofun.API.Helpers.Headers
{
    public class PaginationHeader
    {
        public Int32 CurrentPage { get; set; }
        public Int32 ItemsPerPage { get; set; }
        public Int32 TotalItems { get; set; }
        public Int32 TotalPages { get; set; }

        public PaginationHeader(Int32 currentPage, Int32 itemsPerPage, Int32 totalItems, Int32 totalPages)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }
    }
}