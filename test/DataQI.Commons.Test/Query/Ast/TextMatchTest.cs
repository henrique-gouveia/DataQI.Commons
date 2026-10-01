using DataQI.Commons.Query;
using DataQI.Commons.Query.Ast;

using Moq;
using Xunit;

namespace DataQI.Commons.Test.Query.Ast
{
    public class TextMatchTest
    {
        [Fact]
        public void TestPropertiesReturnConstructorValues()
        {
            var textMatch = new TextMatch("Name", TextMatchKind.StartingWith, "Ad");

            Assert.Equal("Name", textMatch.PropertyName);
            Assert.Equal(TextMatchKind.StartingWith, textMatch.Kind);
            Assert.Equal("Ad", textMatch.Value);
        }

        [Fact]
        public void TestAcceptCallsVisitorWithSelf()
        {
            var textMatch = new TextMatch("Name", TextMatchKind.Like, "Ad");
            var visitor = new Mock<ICriterionVisitor<string>>();
            visitor.Setup(v => v.Visit(textMatch)).Returns("visited");

            Assert.Equal("visited", textMatch.Accept(visitor.Object));
            visitor.Verify(v => v.Visit(textMatch), Times.Once);
        }
    }
}
