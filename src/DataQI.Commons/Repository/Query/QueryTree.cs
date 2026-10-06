using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using DataQI.Commons.Query.Support;
using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Query
{
    public class QueryTree: IEnumerable<QueryTree.Node>
    {
        private static readonly string PrefixPattern = @"\w+By";
        private static readonly string AsyncSuffixPattern = "Async$";
        private static readonly string UppercaseOrEndPattern = "(?=[A-Z]|$)";
        private static readonly string OrderByPattern = $"OrderBy{UppercaseOrEndPattern}";
        private static readonly string DirectionPattern = $"(Asc|Desc){UppercaseOrEndPattern}";

        private static readonly Regex OrderByMatcher = new Regex(OrderByPattern);
        private static readonly Regex DirectionMatcher = new Regex(DirectionPattern);

        private readonly Predicate predicate;
        private readonly OrderPredicate orderPredicate;

        public QueryTree(string source)
        {
            Assert.NotNullOrEmpty(source, "Source must not be null or empty");

            source = Regex.Replace(source, AsyncSuffixPattern, "");

            var orderByMatch = OrderByMatcher.Match(source);
            var orderSource = "";
            if (orderByMatch.Success)
            {
                orderSource = source.Substring(orderByMatch.Index + orderByMatch.Length);
                source = source.Substring(0, orderByMatch.Index);
            }

            orderPredicate = new OrderPredicate(orderSource);

            var match = Regex.Match(source, PrefixPattern);
            predicate = new Predicate(source.Substring(match.Length));
        }

        private static string[] Split(string input, string pattern)
        {
            return Regex.Split(input, pattern, RegexOptions.Compiled);
        }

        public IEnumerator<Node> GetEnumerator() => predicate.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IReadOnlyList<OrderMember> OrderMembers => orderPredicate.OrderMembers;

        private class Predicate: IEnumerable<Node>
        {
            public Predicate(string predicate)
            {
                foreach (var source in Split(predicate, "Or"))
                    nodes.Add(new Node(source));
            }

            private readonly List<Node> nodes = new List<Node>();
            public IEnumerator<Node> GetEnumerator() => nodes.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class OrderPredicate
        {
            public OrderPredicate(string orderSource)
            {
                var currentIndex = 0;
                foreach (Match directionMatch in DirectionMatcher.Matches(orderSource))
                {
                    var propertyName = orderSource.Substring(currentIndex, directionMatch.Index - currentIndex);
                    var direction = directionMatch.Value == "Asc" ? OrderDirection.Asc : OrderDirection.Desc;
                    orderMembers.Add(new OrderMember(propertyName, direction));
                    currentIndex = directionMatch.Index + directionMatch.Length;
                }

                if (currentIndex < orderSource.Length)
                {
                    var propertyName = orderSource.Substring(currentIndex);
                    orderMembers.Add(new OrderMember(propertyName, OrderDirection.Asc));
                }
            }

            private readonly List<OrderMember> orderMembers = new List<OrderMember>();

            public IReadOnlyList<OrderMember> OrderMembers => orderMembers;
        }

        public class Node : IEnumerable<QueryMember>
        {
            public Node(string source)
            {
                foreach (var criterion in Split(source, "And"))
                    members.Add(new QueryMember(criterion));
            }
            
            private readonly List<QueryMember> members = new List<QueryMember>();
            public IEnumerator<QueryMember> GetEnumerator() => members.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class OrderMember
        {
            public OrderMember(string propertyName, OrderDirection direction)
            {
                PropertyName = propertyName;
                Direction = direction;
            }

            public string PropertyName { get; }
            public OrderDirection Direction { get; }
        }
    }
}
