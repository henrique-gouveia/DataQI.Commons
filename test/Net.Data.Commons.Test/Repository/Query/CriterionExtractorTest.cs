using System;
using System.Linq;
using Xunit;

using Net.Data.Commons.Repository.Query;

namespace Net.Data.Commons.Test.Repository.Query
{
    public class CriterionExtractorTest
    {
        [Fact]
        public void TestRejectsNullSource() 
        {
            Assert.Throws<ArgumentException>(() => new CriterionExtractor(null));
        }

        [Fact]
        public void TestExtractSimplePropertyCorrectly()
        {
            var extractor = new CriterionExtractor("FirstName");
            AssertExtractor(extractor, "FirstName");
        }

        private void AssertExtractor(CriterionExtractor extractor, params string[] exptectedCriterions)
        {
            var criterions = extractor.Criterions;
            for (int i = 0; i < exptectedCriterions.Length; i++)
                Assert.Equal(exptectedCriterions[i], criterions[i]);
        }
    }
}