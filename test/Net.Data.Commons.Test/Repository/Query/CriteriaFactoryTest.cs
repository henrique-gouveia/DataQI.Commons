using System;
using Moq;
using Net.Data.Commons.Repository.Query;
using Net.Data.Commons.Test.Repository.Sample;
using Xunit;

using static Net.Data.Commons.Repository.Query.CriterionExtractor;

namespace Net.Data.Commons.Test.Repository.Query
{
    public class CriteriaFactoryTest
    {
        [Fact]
        public void TestRejectsNullCriterions()
        {
            var exception = Assert.Throws<ArgumentException>(() => 
                new CriteriaFactory(null, null));
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Criterions must not be null", exceptionMessage);
        }

        [Fact]
        public void TestRejectsNullMethodParameters()
        {
            var extractor = new CriterionExtractor("FirstName");
            
            var exception = Assert.Throws<ArgumentException>(() => 
                new CriteriaFactory(extractor.GetEnumerator(), null));
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Method Parameters must not be null", exceptionMessage);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyCorrectly()
        {
            var fakeRepository = new Mock<IFakeRepository>().Object;
            var method = fakeRepository.GetType().GetMethod("FindByName");
            var extractor = new CriterionExtractor(method.Name);

            var criteria = new CriteriaFactory(extractor.GetEnumerator(), method.GetParameters()).Create<FakeEntity>();

            Assert.NotNull(criteria);
            Assert.Equal("(Name = @name)", criteria.ToSqlString());
        }
   }
}