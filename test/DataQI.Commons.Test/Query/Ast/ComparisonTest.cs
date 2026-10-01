using DataQI.Commons.Query.Ast;
using ComparisonKind = DataQI.Commons.Query.Ast.Support.ComparisonKind;
using Moq;
using Xunit;

namespace DataQI.Commons.Test.Query.Ast
{
    public class ComparisonTest
    {
        [Fact]
        public void TestPropertiesReturnConstructorValues()
        {
            var comparison = new Comparison("Age", ComparisonKind.GreaterThan, 30);

            Assert.Equal("Age", comparison.PropertyName);
            Assert.Equal(ComparisonKind.GreaterThan, comparison.Kind);
            Assert.Equal(30, comparison.Value);
        }

        [Fact]
        public void TestAcceptCallsVisitorWithSelf()
        {
            var comparison = new Comparison("Age", ComparisonKind.Equal, 30);
            var visitor = new Mock<ICriterionVisitor<string>>();
            visitor.Setup(v => v.Visit(comparison)).Returns("visited");

            Assert.Equal("visited", comparison.Accept(visitor.Object));
            visitor.Verify(v => v.Visit(comparison), Times.Once);
        }
    }
}
