using System.Collections.Generic;

namespace Ma.Shared
{
    /// <summary>
    /// Represents a pagination of entities to indicate a series of related content exists across multiple pages.
    /// </summary>
    /// <typeparam name="TEntity">Type of pagination object.</typeparam>
    public sealed class PaginatedItems<TEntity> where TEntity : class
    {
        /// <summary>
        /// Get amount of items to show.
        /// </summary>
        public int PageSize { get; private set; }
        
        /// <summary>
        /// Get current index of page.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Get the total number of items across all pages.
        /// </summary>
        public long TotalCount { get; private set; }

        /// <summary>
        /// Get the items of data.
        /// </summary>
        public IEnumerable<TEntity> Data { get; private set; }

        /// <summary>
        /// Creates a new PaginatedItems object.
        /// </summary>        
        public PaginatedItems(int pageIndex, int pageSize, long totalCount, IEnumerable<TEntity> data)
        {
            PageIndex  = pageIndex;
            PageSize   = pageSize;
            TotalCount = totalCount;
            Data       = data;
        }
    }
}
