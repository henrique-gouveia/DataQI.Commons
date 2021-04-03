using System;
using Xunit;

using AssertUtil = DataQI.Commons.Util.Assert;

namespace DataQI.Commons.Test.Util
{
    public class AssertTest
    {
        [Fact]
        public void TestAssertIsType()
        {
            var expectedMessage = "Invalid type";

            var objOne = new TypeOne();
            var objTwo = new TypeOne();

            AssertUtil.IsType(objOne.GetType(), objTwo, expectedMessage);
        }

        [Fact]
        public void TestAssertIsTypeThrowsException()
        {
            var expectedMessage = "Invalid type";

            var objOne = new TypeOne();
            var objTwo = new TypeTwo();

            var exception = Assert.Throws<ArgumentException>(() =>
                AssertUtil.IsType(objOne.GetType(), objTwo, expectedMessage));

            AssertExcetion<ArgumentException>(exception, expectedMessage);
        }

        [Fact]
        public void TestAssertIsTypeUsingGenerics()
        {
            var expectedMessage = "Invalid type";
            var objOne = new TypeOne();

            AssertUtil.IsType<TypeOne>(objOne, expectedMessage);
        }

        [Fact]
        public void TestAssertIsTypeUsingGenericsThrowsException()
        {
            var expectedMessage = "Invalid type";

            var objOne = new TypeOne();
            
            var exception = Assert.Throws<ArgumentException>(() =>
                AssertUtil.IsType<TypeTwo>(objOne, expectedMessage));

            AssertExcetion<ArgumentException>(exception, expectedMessage);
        }

        [Fact]
        public void TestAssertTrue()
            => AssertUtil.True(true, "Invalid expression");

        [Fact]
        public void TestAssertTrueThrowsException()
        {
            var expectedMessage = "Invalid expression";
            var exception = Assert.Throws<ArgumentException>(() =>
                AssertUtil.True(false, expectedMessage));

            AssertExcetion<ArgumentException>(exception, expectedMessage);
        }

        [Fact]
        public void TestAssertNotNull()
            => AssertUtil.NotNull(new TypeOne(), "object must not be null");

        [Fact]
        public void TestAssertNotNullThrowsException()
        {
            var expectedMessage = "object must not be null";
            var exception = Assert.Throws<ArgumentException>(() => 
                AssertUtil.NotNull(null, expectedMessage));

            AssertExcetion<ArgumentException>(exception, expectedMessage);
        }

        [Fact]
        public void TestAssertNotNullOrEmpty()
            => AssertUtil.NotNullOrEmpty("Some text", "object must not be null or empty");

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TestAssertNotNullOrEmptyThrowsException(string text)
        {
            var expectedMessage = "object must not be null or empty";
            var exception = Assert.Throws<ArgumentException>(() =>
                AssertUtil.NotNullOrEmpty(text, expectedMessage));

            AssertExcetion<ArgumentException>(exception, expectedMessage);
        }

        private void AssertExcetion<T>(Exception exception, string expectedMessage)
        {
            var baseException = exception.GetBaseException();

            Assert.IsType<T>(baseException);
            Assert.Equal(expectedMessage, baseException.Message);
        }

        private class TypeOne { }
        private class TypeTwo { }
    }
}
