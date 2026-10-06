using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

using DataQI.Commons.Extensions.Reflection;
using DataQI.Commons.Test.Repository.Sample;

namespace DataQI.Commons.Test.Extensions.Reflection
{
    public class TypeExtensionsTest
    {
        [Fact]
        public void TestRecognizesGenericTaskAsAsyncResultType()
        {
            var recognized = typeof(Task<string>).TryGetAsyncResultType(out var resultType);

            Assert.True(recognized);
            Assert.Equal(typeof(string), resultType);
        }

        [Theory]
        [InlineData(typeof(Task))]
        [InlineData(typeof(string))]
        [InlineData(typeof(int))]
        public void TestRejectsNonGenericOrNonTaskTypes(Type type)
        {
            var recognized = type.TryGetAsyncResultType(out var resultType);

            Assert.False(recognized);
            Assert.Null(resultType);
        }

        [Fact]
        public void TestGetAllInterfaceMethodsIncludesInheritedMethods()
        {
            var methodNames = typeof(IFakeRepository).GetAllInterfaceMethods().Select(m => m.Name);

            Assert.Contains(nameof(IFakeRepository.NotImplementedMethod), methodNames);
            Assert.Contains("Insert", methodNames);
            Assert.Contains("FindOne", methodNames);
            Assert.Contains("Find", methodNames);
        }
    }
}
