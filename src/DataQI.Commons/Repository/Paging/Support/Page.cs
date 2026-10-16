using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Paging.Support
{
    /// <summary>Immutable <see cref="IPage{T}"/>; it keeps a read-only copy of the items it receives.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    public sealed class Page<T> : IPage<T>
    {
        /// <summary>Creates a page.</summary>
        /// <param name="items">The items of the page; must not be <c>null</c> nor exceed <paramref name="pageSize"/>.</param>
        /// <param name="pageIndex">The zero-based page index.</param>
        /// <param name="pageSize">The requested page size.</param>
        /// <param name="totalCount">The total number of rows; must not be negative.</param>
        /// <exception cref="ArgumentException">An argument breaks an invariant.</exception>
        public Page(IReadOnlyList<T> items, int pageIndex, int pageSize, long totalCount)
        {
            Assert.NotNull(items, "Items must not be null");
            Assert.True(pageIndex >= 0, "Page index must not be negative");
            Assert.True(pageSize >= 1, "Page size must be at least 1");
            Assert.True(items.Count <= pageSize, "Items must not exceed the page size");
            Assert.True(totalCount >= 0, "Total count must not be negative");

            Items = new ReadOnlyCollection<T>(items.ToArray());
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        /// <inheritdoc />
        public IReadOnlyList<T> Items { get; }
        /// <inheritdoc />
        public int PageIndex { get; }
        /// <inheritdoc />
        public int PageSize { get; }
        /// <inheritdoc />
        public long TotalCount { get; }
        /// <inheritdoc />
        public long TotalPages => TotalCount / PageSize + (TotalCount % PageSize == 0 ? 0 : 1);
        /// <inheritdoc />
        public bool HasNext => (long)PageIndex * PageSize + Items.Count < TotalCount;
        /// <inheritdoc />
        public bool HasPrevious => PageIndex > 0;
    }
}
