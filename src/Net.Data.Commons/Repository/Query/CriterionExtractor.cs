using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Net.Data.Commons.Repository.Query
{
    public class CriterionExtractor
    {
        private readonly Predicate predicate;
        private readonly Regex preffixRegex = new Regex(@"\w+By");

        public CriterionExtractor(string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentException("Source must not be null");

            var match = preffixRegex.Match(source);
            if(match.Length > 0)
                predicate = new Predicate(source.Substring(match.Length));
            else
                predicate = new Predicate(source);
        }

        private static string[] Split(string input, string pattern) 
        {
            var regex = new Regex(pattern, RegexOptions.Compiled);
            return regex.Split(input);
        }

        public IEnumerator<OrCriterion> GetEnumerator()
        {
            return predicate.GetEnumerator();
        }

        public class OrCriterion : IEnumerable<Criterion>
        {
            private readonly List<Criterion> criterions = new List<Criterion>();

            public OrCriterion(string source)
            {   
                foreach (var criterion in Split(source, "And"))
                    criterions.Add(new Criterion(criterion));
            }

            public IEnumerator<Criterion> GetEnumerator()
            {
                return criterions.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class Predicate : IEnumerable<OrCriterion>
        {
            private readonly List<OrCriterion> nodes = new List<OrCriterion>();

            public Predicate(string predicate)
            {
                foreach (var criterion in Split(predicate, "Or")) 
                    nodes.Add(new OrCriterion(criterion));
            }

            public IEnumerator<OrCriterion> GetEnumerator()
            {
                return nodes.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}