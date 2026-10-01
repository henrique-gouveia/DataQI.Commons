using DataQI.Commons.Query.Ast;
using Moq;
using Xunit;

namespace DataQI.Commons.Test.Query.Ast
{
    public class BetweenTest
    {
        [Fact]
        public void TestPropertiesReturnConstructorValues()
        {
            var between = new Between("Age", 18, 65);

            Assert.Equal("Age", between.PropertyName);
            Assert.Equal(18, between.Starts);
            Assert.Equal(65, between.Ends);
        }

        [Fact]
        public void TestAcceptCallsVisitorWithSelf()
        {
            var between = new Between("Age", 18, 65);
            var visitor = new Mock<ICriterionVisitor<string>>();
            visitor.Setup(v => v.Visit(between)).Returns("visited");

            Assert.Equal("visited", between.Accept(visitor.Object));
            visitor.Verify(v => v.Visit(between), Times.Once);
        }
    }
}
