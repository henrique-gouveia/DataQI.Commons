using System;
using System.Collections.Generic;
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

            AssertException<ArgumentException>(exception, expectedMessage);
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

            AssertException<ArgumentException>(exception, expectedMessage);
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

            AssertException<ArgumentException>(exception, expectedMessage);
        }

        [Fact]
        public void TestAssertTruePreservesParameterName()
        {
            AssertUtil.True(true, "Invalid page", "page");

            var exception = Assert.Throws<ArgumentException>(() =>
                AssertUtil.True(false, "Invalid page", "page"));

            Assert.Equal(new ArgumentException("Invalid page", "page").Message, exception.Message);
            Assert.Equal("page", exception.ParamName);
        }

        [Fact]
        public void TestAssertTrueThrowsTheRequestedException()
        {
            AssertUtil.True<InvalidOperationException>(true, "Invalid state");

            var exception = Assert.Throws<InvalidOperationException>(() =>
                AssertUtil.True<InvalidOperationException>(false, "Invalid state"));

            Assert.Equal("Invalid state", exception.Message);
        }

        [Fact]
        public void TestAssertTrueWithRequestedExceptionEvaluatesMessageOnlyOnFailure()
        {
            var evaluations = 0;
            Func<string> message = () =>
            {
                evaluations++;
                return "Invalid state";
            };

            AssertUtil.True<InvalidOperationException>(true, message);
            Assert.Equal(0, evaluations);

            var exception = Assert.Throws<InvalidOperationException>(() => AssertUtil.True<InvalidOperationException>(false, message));

            Assert.Equal("Invalid state", exception.Message);
            Assert.Equal(1, evaluations);
        }

        [Fact]
        public void TestAssertFalseThrowsException()
        {
            AssertUtil.False(false, "Invalid expression");

            var exception = Assert.Throws<ArgumentException>(() =>
                AssertUtil.False(true, "Invalid expression"));

            AssertException<ArgumentException>(exception, "Invalid expression");
        }

        [Fact]
        public void TestAssertFalsePreservesParameterName()
        {
            AssertUtil.False(false, "Invalid expression", "page");

            var exception = Assert.Throws<ArgumentException>(() =>
                AssertUtil.False(true, "Invalid expression", "page"));

            Assert.Equal("page", exception.ParamName);
            Assert.Equal(new ArgumentException("Invalid expression", "page").Message, exception.Message);
        }

        [Fact]
        public void TestAssertFalseThrowsTheRequestedException()
        {
            AssertUtil.False<InvalidOperationException>(false, "Invalid state");

            var exception = Assert.Throws<InvalidOperationException>(() =>
                AssertUtil.False<InvalidOperationException>(true, "Invalid state"));

            Assert.Equal("Invalid state", exception.Message);
        }

        [Fact]
        public void TestAssertFalseWithRequestedExceptionEvaluatesMessageOnlyOnFailure()
        {
            var evaluations = 0;
            Func<string> message = () =>
            {
                evaluations++;
                return "Invalid state";
            };

            AssertUtil.False<InvalidOperationException>(false, message);
            Assert.Equal(0, evaluations);

            var exception = Assert.Throws<InvalidOperationException>(() =>
                AssertUtil.False<InvalidOperationException>(true, message));

            Assert.Equal("Invalid state", exception.Message);
            Assert.Equal(1, evaluations);
        }

        [Fact]
        public void TestAssertNotNullWithRequestedExceptionEvaluatesMessageOnlyOnFailure()
        {
            var evaluations = 0;
            Func<string> message = () =>
            {
                evaluations++;
                return "Missing";
            };

            AssertUtil.NotNull<InvalidOperationException>(new TypeOne(), message);
            Assert.Equal(0, evaluations);

            var exception = Assert.Throws<InvalidOperationException>(() =>
                AssertUtil.NotNull<InvalidOperationException>(null, message));

            Assert.Equal("Missing", exception.Message);
            Assert.Equal(1, evaluations);
        }

        [Fact]
        public void TestAssertNotEmptyWithRequestedException()
        {
            AssertUtil.NotEmpty<InvalidOperationException>(new List<int> { 1 }, "Empty");
            AssertUtil.NotEmpty<InvalidOperationException>(new List<int> { 1 }, () => "Empty");

            Assert.Equal("Empty", Assert.Throws<InvalidOperationException>(() =>
                AssertUtil.NotEmpty<InvalidOperationException>(new List<int>(), "Empty")).Message);
            Assert.Equal("Empty", Assert.Throws<InvalidOperationException>(() =>
                AssertUtil.NotEmpty<InvalidOperationException>(new List<int>(), () => "Empty")).Message);
        }

        [Fact]
        public void TestAssertNotNull()
            => AssertUtil.NotNull(new TypeOne(), "object must not be null");

        [Fact]
        public void TestAssertNotNullThrowsException()
        {
            var expectedMessage = "Object must not be null";
            var exception = Assert.Throws<ArgumentException>(() => 
                AssertUtil.NotNull(null, expectedMessage));

            AssertException<ArgumentException>(exception, expectedMessage);
        }

        [Fact]
        public void TestAssertNotNullPreservesParameterName()
        {
            AssertUtil.NotNull(new object(), "Missing rows", "rows");

            var exception = Assert.Throws<ArgumentException>(() =>
                AssertUtil.NotNull(null, "Missing rows", "rows"));

            Assert.Equal(new ArgumentException("Missing rows", "rows").Message, exception.Message);
            Assert.Equal("rows", exception.ParamName);
        }

        [Fact]
        public void TestAssertNotNullOrEmpty()
            => AssertUtil.NotNullOrEmpty("Some text", "Object must not be null or empty");

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TestAssertNotNullOrEmptyThrowsException(string text)
        {
            var expectedMessage = "Object must not be null or empty";
            var exception = Assert.Throws<ArgumentException>(() =>
                AssertUtil.NotNullOrEmpty(text, expectedMessage));

            AssertException<ArgumentException>(exception, expectedMessage);
        }

        private void AssertException<T>(Exception exception, string expectedMessage)
        {
            var baseException = exception.GetBaseException();

            Assert.IsType<T>(baseException);
            Assert.Equal(expectedMessage, baseException.Message);
        }

        private class TypeOne { }
        private class TypeTwo { }
    }
}
