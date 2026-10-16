using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using DataQI.Commons.Query;
using DataQI.Commons.Query.Support;
using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Paging.Support
{
    /// <summary>Immutable <see cref="IPageRequest"/>; it keeps immutable copies of the orderings it receives.</summary>
    public sealed class PageRequest : IPageRequest
    {
        private PageRequest(int pageIndex, int pageSize, IReadOnlyList<IOrderCriterion> orders)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Orders = orders;
        }

        /// <inheritdoc />
        public int PageIndex { get; }
        /// <inheritdoc />
        public int PageSize { get; }
        /// <inheritdoc />
        public long Offset => (long)PageIndex * PageSize;
        /// <inheritdoc />
        public IReadOnlyList<IOrderCriterion> Orders { get; }

        /// <summary>Creates a page request.</summary>
        /// <param name="pageIndex">The zero-based page index; must not be negative.</param>
        /// <param name="pageSize">The page size; at least 1 and less than <see cref="int.MaxValue"/>.</param>
        /// <param name="orders">Extra orderings; none of them may be <c>null</c>.</param>
        /// <exception cref="ArgumentException">An argument is out of range.</exception>
        public static PageRequest Of(int pageIndex, int pageSize, params IOrderCriterion[] orders)
        {
            Assert.True(pageIndex >= 0, "Page index must not be negative");
            Assert.True(pageSize >= 1, "Page size must be at least 1");
            Assert.True(pageSize < int.MaxValue, "Page size must be less than int.MaxValue");

            var source = orders ?? Array.Empty<IOrderCriterion>();
            Assert.True(source.All(o => o != null), "Orders must not contain null");

            var copy = source
                .Select(o => (IOrderCriterion)new OrderCriterion(o.PropertyName, o.Direction))
                .ToArray();

            return new PageRequest(pageIndex, pageSize, new ReadOnlyCollection<IOrderCriterion>(copy));
        }
    }
}
