using System;
using Net.Data.Commons.Repository.Query;
using Xunit;

using static Net.Data.Commons.Repository.Query.CriterionExtractor;

namespace Net.Data.Commons.Test.Repository.Query
{
    public class CriterionSqlWhereBuilderTest
    {
        [Fact]
        public void TestRejectsNullCriterions()
        {
            var exception = Assert.Throws<ArgumentException>(() => new CriterionSqlWhereBuilder(null, null));
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Criterions must not be null", exceptionMessage);
        }

        [Fact]
        public void TestRejectsNullMethodParameters()
        {
            var extractor = new CriterionExtractor("FirstName");
            
            var exception = Assert.Throws<ArgumentException>(() => 
                new CriterionSqlWhereBuilder(extractor.GetEnumerator(), null));
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Method Parameters must not be null", exceptionMessage);
        }
   }
}