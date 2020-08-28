using System.Collections;

namespace DataQI.Commons.Extensions.Collections
{
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