using DataQI.Commons.Query.Ast;
using Moq;
using Xunit;

namespace DataQI.Commons.Test.Query.Ast
{
    public class InTest
    {
        [Fact]
        public void TestPropertiesReturnConstructorValues()
        {
            var values = new object[] { "Fortaleza", "Barcelona" };
            var inCriterion = new In("City", values);

            Assert.Equal("City", inCriterion.PropertyName);
            Assert.Equal(values, inCriterion.Values);
        }

        [Fact]
        public void TestAcceptCallsVisitorWithSelf()
        {
            var inCriterion = new In("City", new object[] { "Fortaleza" });
            var visitor = new Mock<ICriterionVisitor<string>>();
            visitor.Setup(v => v.Visit(inCriterion)).Returns("visited");

            Assert.Equal("visited", inCriterion.Accept(visitor.Object));
            visitor.Verify(v => v.Visit(inCriterion), Times.Once);
        }
    }
}
