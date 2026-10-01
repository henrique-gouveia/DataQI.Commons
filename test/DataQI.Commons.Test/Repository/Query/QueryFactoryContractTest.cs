using System;
using System.Reflection;

using DataQI.Commons.Repository.Query;
using DataQI.Commons.Test.Repository.Sample;

using Moq;
using Xunit;

namespace DataQI.Commons.Test.Repository.Query
{
    public class QueryFactoryContractTest
    {
        [Theory]
        [MemberData(nameof(QueryContractCases.All), MemberType = typeof(QueryContractCases))]
        public void TestLegacyParserContract(QueryContractCase testCase)
        {
            var method = QueryMethod(testCase.MethodName);

            if (testCase.ExpectsException)
            {
                var exception = Assert.Throws<ArgumentException>(() =>
                    new QueryFactory(method, testCase.Arguments).CreateCriteria());
                Assert.Equal(testCase.ExpectedExceptionMessage, exception.GetBaseException().Message);
            }
            else
            {
                var criteria = new QueryFactory(method, testCase.Arguments).CreateCriteria();
                Assert.NotNull(criteria);
            }
        }

        private MethodInfo QueryMethod(string name)
        {
            var fakeRepository = new Mock<IFakeRepository>().Object;
            return fakeRepository.GetType().GetMethod(name);
        }
    }
}
