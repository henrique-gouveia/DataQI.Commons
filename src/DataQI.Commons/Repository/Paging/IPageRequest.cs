using System.Collections.Generic;

using DataQI.Commons.Query;

namespace DataQI.Commons.Repository.Paging
{
    /// <summary>Describes the page to fetch: zero-based index, size and optional extra ordering.</summary>
    public interface IPageRequest
    {
        /// <summary>Gets the zero-based index of the page.</summary>
        int PageIndex { get; }
        /// <summary>Gets the maximum number of items in a page; at least 1.</summary>
        int PageSize { get; }
        /// <summary>Gets the number of rows skipped: <c>PageIndex * PageSize</c>.</summary>
        long Offset { get; }
        /// <summary>Gets the orderings applied after those of the method name and of the criteria.</summary>
        IReadOnlyList<IOrderCriterion> Orders { get; }
    }
}
