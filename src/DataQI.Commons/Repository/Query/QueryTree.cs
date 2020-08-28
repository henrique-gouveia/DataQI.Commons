using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Query
{
    public class QueryTree : IEnumerable<QueryTree.Node>
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

        public IEnumerator<QueryTree.Node> GetEnumerator() => predicate.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class Predicate : IEnumerable<Node>
        {
            private readonly ICollection<Node> nodes = new List<Node>();

            public Predicate(string predicate)
            {
                foreach (var source in Split(predicate, "Or"))
                    nodes.Add(new Node(source));
            }

            public IEnumerator<Node> GetEnumerator() => nodes.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class Node : IEnumerable<QueryMember>
        {
            private readonly ICollection<QueryMember> members = new List<QueryMember>();

            public Node(string source)
            {
                foreach (var criterion in Split(source, "And"))
                    members.Add(new QueryMember(criterion));
            }

            public IEnumerator<QueryMember> GetEnumerator() => members.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}