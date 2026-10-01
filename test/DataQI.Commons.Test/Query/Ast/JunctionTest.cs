using System;
using System.Linq;

using DataQI.Commons.Query.Ast;
using LogicalKind = DataQI.Commons.Query.Ast.Support.LogicalKind;

using Moq;
using Xunit;

namespace DataQI.Commons.Test.Query.Ast
{
    public class JunctionTest
    {
        [Fact]
        public void TestAddRejectsNull()
        {
            var junction = new Junction(LogicalKind.And);

            var exception = Assert.Throws<ArgumentException>(() => junction.Add(null));

            Assert.Equal("Criterion must not be null", exception.GetBaseException().Message);
        }

        [Fact]
        public void TestKindReturnsConstructorValue()
        {
            var junction = new Junction(LogicalKind.Or);

            Assert.Equal(LogicalKind.Or, junction.Kind);
        }

        [Fact]
        public void TestAddAppendsToMembersInCallSequence()
        {
            var junction = new Junction(LogicalKind.And);
            var first = new IsNull("Email");
            var second = new IsNull("Phone");

            junction.Add(first).Add(second);

            Assert.Equal(2, junction.Members.Count);
            Assert.Same(first, junction.Members.ElementAt(0));
            Assert.Same(second, junction.Members.ElementAt(1));
        }

        [Fact]
        public void TestAddReturnsSameJunctionForChaining()
        {
            var junction = new Junction(LogicalKind.And);

            Assert.Same(junction, junction.Add(new IsNull("Email")));
        }

        [Fact]
        public void TestAcceptCallsVisitorWithSelf()
        {
            var junction = new Junction(LogicalKind.And);
            var visitor = new Mock<ICriterionVisitor<string>>();
            visitor.Setup(v => v.Visit(junction)).Returns("visited");

            Assert.Equal("visited", junction.Accept(visitor.Object));
            visitor.Verify(v => v.Visit(junction), Times.Once);
        }
    }
}
