using System.Collections.Generic;
using System.Text.RegularExpressions;

using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Query
{
    public class QueryTree
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

        public IReadOnlyCollection<Node> Nodes => predicate.Nodes; 

        private class Predicate
        {
            public Predicate(string predicate)
            {
                foreach (var source in Split(predicate, "Or"))
                    nodes.Add(new Node(source));
            }

            private readonly List<Node> nodes = new List<Node>();
            public IReadOnlyCollection<Node> Nodes => nodes;
        }

        public class Node
        {
            public Node(string source)
            {
                foreach (var criterion in Split(source, "And"))
                    members.Add(new QueryMember(criterion));
            }
            
            private readonly List<QueryMember> members = new List<QueryMember>();
            public IReadOnlyCollection<QueryMember> Members => members;
        }
    }
}