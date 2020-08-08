using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Query
{
    public class QueryTree : IEnumerable<QueryTree.OrQueryMember>
    {
        private static readonly string PrefixPattern = @"\w+By";

        private readonly Predicate predicate;

        public QueryTree(string source)
        {
            Assert.NotNullOrEmpty(source, "Source must not be null or empty");

            var match = Regex.Match(source, PrefixPattern);
            predicate = new Predicate(source.Substring(match.Length));
        }

        private static string[] Split(string input, string pattern)
        {
            return Regex.Split(input, pattern, RegexOptions.Compiled);
        }

        #region IEnumerable<OrCriterion> implementations
        public IEnumerator<QueryTree.OrQueryMember> GetEnumerator()
        {
            return predicate.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        private class Predicate : IEnumerable<OrQueryMember>
        {
            private readonly ICollection<OrQueryMember> nodes = new List<OrQueryMember>();

            public Predicate(string predicate)
            {
                foreach (var source in Split(predicate, "Or"))
                    nodes.Add(new OrQueryMember(source));
            }

            #region IEnumerable<OrQueryMember> implementations
            public IEnumerator<OrQueryMember> GetEnumerator()
            {
                return nodes.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            #endregion
        }

        public class OrQueryMember : IEnumerable<QueryMember>
        {
            private readonly ICollection<QueryMember> members = new List<QueryMember>();

            public OrQueryMember(string source)
            {
                foreach (var criterion in Split(source, "And"))
                    members.Add(new QueryMember(criterion));
            }

            #region IEnumerable<QueryMember> implementations
            public IEnumerator<QueryMember> GetEnumerator()
            {
                return members.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            #endregion
        }
    }
}