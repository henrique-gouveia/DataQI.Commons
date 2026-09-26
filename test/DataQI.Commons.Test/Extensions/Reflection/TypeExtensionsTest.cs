using System;
using System.Threading.Tasks;

using Xunit;

using DataQI.Commons.Extensions.Reflection;

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
    }
}
