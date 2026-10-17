using System.Collections.Generic;

namespace DataQI.Commons.Repository.Paging
{
    /// <summary>A page of results that knows whether another page follows, without counting rows.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    public interface ISlice<out T>
    {
        /// <summary>Gets the items of the page; never more than <see cref="PageSize"/>.</summary>
        IReadOnlyList<T> Items { get; }
        /// <summary>Gets the zero-based index of the page.</summary>
        int PageIndex { get; }
        /// <summary>Gets the requested page size.</summary>
        int PageSize { get; }
        /// <summary>Gets a value indicating whether a next page exists.</summary>
        bool HasNext { get; }
        /// <summary>Gets a value indicating whether a previous page exists (<c>PageIndex &gt; 0</c>).</summary>
        bool HasPrevious { get; }
    }
}
