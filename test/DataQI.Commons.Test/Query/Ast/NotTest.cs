using DataQI.Commons.Query.Ast;
using Moq;
using Xunit;

namespace DataQI.Commons.Test.Query.Ast
{
    public class NotTest
    {
        [Fact]
        public void TestInnerReturnsConstructorValue()
        {
            var inner = new IsNull("Email");
            var not = new Not(inner);

            Assert.Same(inner, not.Inner);
        }

        [Fact]
        public void TestAcceptCallsVisitorWithSelf()
        {
            var not = new Not(new IsNull("Email"));
            var visitor = new Mock<ICriterionVisitor<string>>();
            visitor.Setup(v => v.Visit(not)).Returns("visited");

            Assert.Equal("visited", not.Accept(visitor.Object));
            visitor.Verify(v => v.Visit(not), Times.Once);
        }
    }
}
