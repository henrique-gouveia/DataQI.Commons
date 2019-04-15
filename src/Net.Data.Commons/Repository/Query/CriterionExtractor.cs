using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Net.Data.Commons.Criterions.Support;
using Net.Data.Commons.Util;

namespace Net.Data.Commons.Repository.Query
{
    public class CriterionExtractor : IEnumerable<CriterionExtractor.OrCriterion>
    {
        private readonly Predicate predicate;
        private readonly Regex preffixRegex = new Regex(@"\w+By");

        public CriterionExtractor(string source)
        {
            Assert.NotNullOrEmpty(source, "Source must not be null or empty");
            
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

        #region IEnumerable<OrCriterion> implementations
        public IEnumerator<CriterionExtractor.OrCriterion> GetEnumerator()
        {
            return predicate.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        private class Predicate : IEnumerable<OrCriterion>
        {
            private readonly List<OrCriterion> nodes = new List<OrCriterion>();

            public Predicate(string predicate)
            {
                foreach (var criterion in Split(predicate, "Or")) 
                    nodes.Add(new OrCriterion(criterion));
            }

            #region IEnumerable<OrCriterion> implementations
            public IEnumerator<OrCriterion> GetEnumerator()
            {
                return nodes.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            #endregion
        }

        public class OrCriterion : IEnumerable<Criterion>
        {
            private readonly List<Criterion> criterions = new List<Criterion>();

            public OrCriterion(string source)
            {   
                foreach (var criterion in Split(source, "And"))
                    criterions.Add(new Criterion(criterion));
            }

            #region IEnumerable<Criterion> implementations
            public IEnumerator<Criterion> GetEnumerator()
            {
                return criterions.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            #endregion
        }        
    }
}