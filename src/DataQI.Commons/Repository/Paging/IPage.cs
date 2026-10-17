namespace DataQI.Commons.Repository.Paging
{
    /// <summary>A page of results that also knows the total number of rows.</summary>
    /// <typeparam name="T">The item type.</typeparam>
    public interface IPage<out T> : ISlice<T>
    {
        /// <summary>Gets the total number of rows matching the criteria.</summary>
        long TotalCount { get; }
        /// <summary>Gets the total number of pages: <c>ceil(TotalCount / PageSize)</c>.</summary>
        long TotalPages { get; }
    }
}
