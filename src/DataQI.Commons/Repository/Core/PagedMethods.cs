using System.Reflection;

namespace DataQI.Commons.Repository.Core
{
    /// <summary>Groups the implementation methods used for paging dispatch.</summary>
    public sealed class PagedMethods
    {
        /// <summary>Creates a group of paging implementation methods.</summary>
        public PagedMethods(MethodInfo findSlice, MethodInfo findSliceAsync, MethodInfo findPage, MethodInfo findPageAsync)
        {
            FindSlice = findSlice;
            FindSliceAsync = findSliceAsync;
            FindPage = findPage;
            FindPageAsync = findPageAsync;
        }

        /// <summary>Gets an empty group for providers without paging support.</summary>
        public static PagedMethods None { get; } = new PagedMethods(null, null, null, null);
        /// <summary>Gets the synchronous slice method.</summary>
        public MethodInfo FindSlice { get; }
        /// <summary>Gets the asynchronous slice method.</summary>
        public MethodInfo FindSliceAsync { get; }
        /// <summary>Gets the synchronous page method.</summary>
        public MethodInfo FindPage { get; }
        /// <summary>Gets the asynchronous page method.</summary>
        public MethodInfo FindPageAsync { get; }
    }
}
