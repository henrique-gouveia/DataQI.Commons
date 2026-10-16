using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Paging
{
    /// <summary>Immutable <see cref="ISlice{T}"/>; it keeps a read-only copy of the items it receives.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    public sealed class Slice<T> : ISlice<T>
    {
        /// <summary>Creates a slice.</summary>
        /// <param name="items">The items of the page; must not be <c>null</c> nor exceed <paramref name="pageSize"/>.</param>
        /// <param name="pageIndex">The zero-based page index.</param>
        /// <param name="pageSize">The requested page size.</param>
        /// <param name="hasNext">Whether a next page exists.</param>
        /// <exception cref="ArgumentException">An argument breaks an invariant.</exception>
        public Slice(IReadOnlyList<T> items, int pageIndex, int pageSize, bool hasNext)
        {
            Assert.NotNull(items, "Items must not be null");
            Assert.True(pageIndex >= 0, "Page index must not be negative");
            Assert.True(pageSize >= 1, "Page size must be at least 1");
            Assert.True(items.Count <= pageSize, "Items must not exceed the page size");

            Items = new ReadOnlyCollection<T>(items.ToArray());
            PageIndex = pageIndex;
            PageSize = pageSize;
            HasNext = hasNext;
        }

        /// <inheritdoc />
        public IReadOnlyList<T> Items { get; }
        /// <inheritdoc />
        public int PageIndex { get; }
        /// <inheritdoc />
        public int PageSize { get; }
        /// <inheritdoc />
        public bool HasNext { get; }
        /// <inheritdoc />
        public bool HasPrevious => PageIndex > 0;
    }
}
