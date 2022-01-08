using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DataQI.Commons.Extensions.Collections
{
    [ExcludeFromCodeCoverage]
    public static class IEnumeratorExtenstions
    {
        public static object NextValue(this IEnumerator enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }

        public static T NextValue<T>(this IEnumerator enumerator)
            => ((T)NextValue(enumerator));
    }
}