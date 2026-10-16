using System.Reflection;

namespace DataQI.Commons.Repository.Core
{
    /// <summary>Groups the <c>Find*</c> methods of the default repository that query methods are dispatched to.</summary>
    public sealed class FindMethods
    {
        /// <summary>Creates a group of repository find methods; any of them may be <c>null</c> when the repository does not implement it.</summary>
        /// <param name="find">The <c>Find(Func&lt;ICriteria, ICriteria&gt;)</c> method.</param>
        /// <param name="findAsync">The <c>FindAsync</c> method taking criteria and a cancellation token.</param>
        /// <param name="findOne">The <c>FindOne(Func&lt;ICriteria, ICriteria&gt;)</c> method.</param>
        /// <param name="findOneAsync">The <c>FindOneAsync</c> method taking criteria and a cancellation token.</param>
        /// <param name="findSlice">The <c>FindSlice</c> method.</param>
        /// <param name="findSliceAsync">The <c>FindSliceAsync</c> method.</param>
        /// <param name="findPage">The <c>FindPage</c> method.</param>
        /// <param name="findPageAsync">The <c>FindPageAsync</c> method.</param>
        public FindMethods(
            MethodInfo find,
            MethodInfo findAsync,
            MethodInfo findOne,
            MethodInfo findOneAsync,
            MethodInfo findSlice = null,
            MethodInfo findSliceAsync = null,
            MethodInfo findPage = null,
            MethodInfo findPageAsync = null)
        {
            Find = find;
            FindAsync = findAsync;
            FindOne = findOne;
            FindOneAsync = findOneAsync;
            FindSlice = findSlice;
            FindSliceAsync = findSliceAsync;
            FindPage = findPage;
            FindPageAsync = findPageAsync;
        }

        /// <summary>Gets a group with no methods.</summary>
        public static FindMethods None { get; } = new FindMethods(null, null, null, null);
        /// <summary>Gets the synchronous collection method.</summary>
        public MethodInfo Find { get; }
        /// <summary>Gets the asynchronous collection method.</summary>
        public MethodInfo FindAsync { get; }
        /// <summary>Gets the synchronous single entity method.</summary>
        public MethodInfo FindOne { get; }
        /// <summary>Gets the asynchronous single entity method.</summary>
        public MethodInfo FindOneAsync { get; }
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
