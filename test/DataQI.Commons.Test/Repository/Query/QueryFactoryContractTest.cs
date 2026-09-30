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
        public void TestContractCase(QueryContractCase testCase)
        {
            var method = QueryMethod(testCase.MethodName);
            var values = QueryValues(method);

            if (testCase.ExpectsException)
            {
                var exception = Assert.Throws<ArgumentException>(() =>
                    new QueryFactory(method, values).CreateCriteria());
                Assert.Equal(testCase.ExpectedExceptionMessage, exception.GetBaseException().Message);
            }
            else
            {
                var criteria = new QueryFactory(method, values).CreateCriteria();
                Assert.NotNull(criteria);
            }
        }

        private MethodInfo QueryMethod(string name)
        {
            var fakeRepository = new Mock<IFakeRepository>().Object;
            return fakeRepository.GetType().GetMethod(name);
        }

        private object[] QueryValues(MethodInfo method)
        {
            var parameters = method.GetParameters();
            var values = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                if (parameterType == typeof(string))
                {
                    values[i] = string.Empty;
                }
                else if (parameterType.IsArray)
                {
                    values[i] = Array.CreateInstance(parameterType.GetElementType(), 0);
                }
                else if (parameterType.IsValueType)
                {
                    values[i] = Activator.CreateInstance(parameterType);
                }
            }

            return values;
        }
    }
}
