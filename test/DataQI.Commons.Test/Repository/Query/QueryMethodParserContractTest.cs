using System.Reflection;

using DataQI.Commons.Repository.Query;
using DataQI.Commons.Test.Repository.Sample;

using ExpectedObjects;
using Moq;
using Xunit;

namespace DataQI.Commons.Test.Repository.Query
{
    public class QueryMethodParserContractTest
    {
        [Theory]
        [MemberData(nameof(QueryContractCases.All), MemberType = typeof(QueryContractCases))]
        public void TestRedesignedParserContract(QueryContractCase testCase)
        {
            var method = QueryMethod(testCase.MethodName);

            var actual = QueryMethodParser.Parse(method).BuildPredicate(testCase.Arguments);

            QueryRedesignedContractExpectations.For(testCase.MethodName).ToExpectedObject().ShouldEqual(actual);
        }

        private MethodInfo QueryMethod(string name)
        {
            var fakeRepository = new Mock<IFakeRepository>().Object;
            return fakeRepository.GetType().GetMethod(name);
        }
    }
}
