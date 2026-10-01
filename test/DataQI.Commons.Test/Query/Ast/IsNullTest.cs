using DataQI.Commons.Query;
using DataQI.Commons.Query.Ast;

using Moq;
using Xunit;

namespace DataQI.Commons.Test.Query.Ast
{
    public class IsNullTest
    {
        [Fact]
        public void TestPropertyNameReturnsConstructorValue()
        {
            var isNull = new IsNull("Email");

            Assert.Equal("Email", isNull.PropertyName);
        }

        [Fact]
        public void TestAcceptCallsVisitorWithSelf()
        {
            var isNull = new IsNull("Email");
            var visitor = new Mock<ICriterionVisitor<string>>();
            visitor.Setup(v => v.Visit(isNull)).Returns("visited");

            Assert.Equal("visited", isNull.Accept(visitor.Object));
            visitor.Verify(v => v.Visit(isNull), Times.Once);
        }
    }
}
