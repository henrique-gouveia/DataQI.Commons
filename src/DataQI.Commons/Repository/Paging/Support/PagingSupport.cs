using System;
using System.Collections.Generic;
using System.Linq;

using DataQI.Commons.Query;
using DataQI.Commons.Query.Support;
using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Paging.Support
{
    /// <summary>Provides ORM-independent validation, ordering and result rules for paging repositories.</summary>
    public static class PagingSupport
    {
        /// <summary>Validates a request, including custom implementations, and rejects offsets above int.MaxValue.</summary>
        /// <param name="page">The request to validate.</param>
        /// <exception cref="ArgumentException">The request violates the paging contract.</exception>
        public static void EnsurePage(IPageRequest page)
        {
            Assert.NotNull(page, "Page must not be null", nameof(page));
            Assert.True(page.PageIndex >= 0 && page.PageSize >= 1 && page.PageSize != int.MaxValue,
                "Page index must not be negative and page size must be between 1 and int.MaxValue - 1", nameof(page));
            Assert.True(page.Offset == (long)page.PageIndex * page.PageSize && page.Offset <= int.MaxValue,
                "Page offset must equal index times size and must not exceed int.MaxValue", nameof(page));
            Assert.NotNull(page.Orders, "Page orders must contain valid property names and directions", nameof(page));
            Assert.False(page.Orders.Any(o => o == null || string.IsNullOrWhiteSpace(o.PropertyName) ||
                (o.Direction != OrderDirection.Asc && o.Direction != OrderDirection.Desc)),
                "Page orders must contain valid property names and directions", nameof(page));
        }

        /// <summary>Merges method/criteria orders, request orders and ascending missing keys; the first property occurrence wins.</summary>
        /// <typeparam name="TEntity">The entity type used in error messages.</typeparam>
        /// <param name="criteriaOrders">Method-name orders followed by explicit criteria orders.</param>
        /// <param name="page">The page request with additional orders.</param>
        /// <param name="keyProperties">The primary-key property names, in key order.</param>
        /// <returns>The complete stable ordering.</returns>
        /// <exception cref="InvalidOperationException">No key or explicit order is available.</exception>
        public static IReadOnlyList<IOrderCriterion> MergeOrders<TEntity>(IEnumerable<IOrderCriterion> criteriaOrders,
            IPageRequest page, IEnumerable<string> keyProperties)
        {
            EnsurePage(page);
            var merged = new List<IOrderCriterion>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var order in criteriaOrders.Concat(page.Orders))
                if (seen.Add(order.PropertyName))
                    merged.Add(order);
            foreach (var key in keyProperties)
                if (seen.Add(key))
                    merged.Add(Order.Asc(key));
            Assert.NotEmpty<InvalidOperationException>(merged,
                () => $"Cannot page {typeof(TEntity).Name}: it has no primary key and no explicit ordering");
            return merged;
        }

        /// <summary>Returns a total only for a partial page with items or an empty first page.</summary>
        /// <param name="itemCount">The number of returned items.</param>
        /// <param name="page">The page request.</param>
        /// <returns>The known total, or null when a count query is required.</returns>
        public static long? DeduceTotal(int itemCount, IPageRequest page)
        {
            EnsurePage(page);
            return itemCount < page.PageSize && (itemCount > 0 || page.PageIndex == 0)
                ? page.Offset + itemCount : (long?)null;
        }

        /// <summary>Returns the number of rows to fetch, including one extra row for a slice.</summary>
        /// <param name="page">The request to validate.</param>
        /// <param name="forSlice">Whether to fetch an extra row to detect the next slice.</param>
        /// <returns>The number of rows to fetch.</returns>
        /// <exception cref="ArgumentException">The request violates the paging contract.</exception>
        public static int FetchSize(IPageRequest page, bool forSlice)
        {
            EnsurePage(page);
            return page.PageSize + (forSlice ? 1 : 0);
        }

        /// <summary>Builds an immutable slice, omitting an extra row used to detect the next page without modifying the source.</summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="rows">The fetched rows, including any extra row.</param>
        /// <param name="page">The page request.</param>
        /// <returns>The requested slice and its navigation metadata.</returns>
        public static ISlice<T> ToSlice<T>(IReadOnlyList<T> rows, IPageRequest page)
        {
            var fetchSize = FetchSize(page, forSlice: true);
            Assert.NotNull(rows, "Rows must not be null", nameof(rows));
            var hasNext = rows.Count >= fetchSize;
            var items = hasNext ? rows.Take(page.PageSize).ToArray() : rows;
            return new Slice<T>(items, page.PageIndex, page.PageSize, hasNext);
        }

        /// <summary>Builds an immutable page from its items and resolved total.</summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The fetched page items.</param>
        /// <param name="page">The page request.</param>
        /// <param name="total">The resolved total number of rows.</param>
        /// <returns>The requested page and its navigation metadata.</returns>
        public static IPage<T> ToPage<T>(IReadOnlyList<T> items, IPageRequest page, long total)
        {
            EnsurePage(page);
            Assert.NotNull(items, "Items must not be null", nameof(items));
            return new Page<T>(items, page.PageIndex, page.PageSize, total);
        }
    }
}
