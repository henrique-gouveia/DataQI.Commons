
using DataQI.Commons.Query;
using DataQI.Commons.Query.Ast;
using DataQI.Commons.Query.Support;
using Xunit;

namespace DataQI.Commons.Test.Query.Support
{
    public class RestrictionsTest
    {
        [Fact]
        public void TestBetweenReturnsBetweenNode()
        {
            var between = Assert.IsType<Between>(Restrictions.Between("Age", 18, 65));

            Assert.Equal("Age", between.PropertyName);
            Assert.Equal(18, between.Starts);
            Assert.Equal(65, between.Ends);
        }

        [Fact]
        public void TestContainingReturnsTextMatchNode()
            => AssertTextMatch(Restrictions.Containing("Title", "General"), "Title", TextMatchKind.Containing, "General");

        [Fact]
        public void TestEndingWithReturnsTextMatchNode()
            => AssertTextMatch(Restrictions.EndingWith("Title", "IT"), "Title", TextMatchKind.EndingWith, "IT");

        [Fact]
        public void TestLikeReturnsTextMatchNode()
            => AssertTextMatch(Restrictions.Like("Title", "General"), "Title", TextMatchKind.Like, "General");

        [Fact]
        public void TestStartingWithReturnsTextMatchNodeAndTakesString()
            => AssertTextMatch(Restrictions.StartingWith("Title", "IT"), "Title", TextMatchKind.StartingWith, "IT");

        [Fact]
        public void TestEqualReturnsComparisonNode()
            => AssertComparison(Restrictions.Equal("FirstName", "Adams"), "FirstName", ComparisonKind.Equal, "Adams");

        [Fact]
        public void TestGreaterThanReturnsComparisonNode()
            => AssertComparison(Restrictions.GreaterThan("Age", 30), "Age", ComparisonKind.GreaterThan, 30);

        [Fact]
        public void TestGreaterThanEqualReturnsComparisonNode()
            => AssertComparison(Restrictions.GreaterThanEqual("Age", 30), "Age", ComparisonKind.GreaterThanEqual, 30);

        [Fact]
        public void TestLessThanReturnsComparisonNode()
            => AssertComparison(Restrictions.LessThan("Age", 30), "Age", ComparisonKind.LessThan, 30);

        [Fact]
        public void TestLessThanEqualReturnsComparisonNode()
            => AssertComparison(Restrictions.LessThanEqual("Age", 30), "Age", ComparisonKind.LessThanEqual, 30);

        [Fact]
        public void TestInReturnsInNode()
        {
            var cities = new object[] { "Fortaleza", "Barcelona" };

            var inCriterion = Assert.IsType<In>(Restrictions.In("City", cities));

            Assert.Equal("City", inCriterion.PropertyName);
            Assert.Equal(cities, inCriterion.Values);
        }

        [Fact]
        public void TestNotReturnsNotNodeWrappingTheInnerCriterion()
        {
            var inner = Restrictions.Equal("FirstName", "Adams");

            var not = Assert.IsType<Not>(Restrictions.Not(inner));

            Assert.Same(inner, not.Inner);
        }

        [Fact]
        public void TestNullReturnsIsNullNode()
        {
            var isNull = Assert.IsType<IsNull>(Restrictions.Null("Email"));

            Assert.Equal("Email", isNull.PropertyName);
        }

        [Fact]
        public void TestConjunctionReturnsAndJunction()
            => Assert.Equal(LogicalKind.And, Restrictions.Conjunction().Kind);

        [Fact]
        public void TestDisjunctionReturnsOrJunction()
            => Assert.Equal(LogicalKind.Or, Restrictions.Disjunction().Kind);

        private static void AssertTextMatch(ICriterion criterion, string property, TextMatchKind kind, string value)
        {
            var textMatch = Assert.IsType<TextMatch>(criterion);
            Assert.Equal(property, textMatch.PropertyName);
            Assert.Equal(kind, textMatch.Kind);
            Assert.Equal(value, textMatch.Value);
        }

        private static void AssertComparison(ICriterion criterion, string property, ComparisonKind kind, object value)
        {
            var comparison = Assert.IsType<Comparison>(criterion);
            Assert.Equal(property, comparison.PropertyName);
            Assert.Equal(kind, comparison.Kind);
            Assert.Equal(value, comparison.Value);
        }
    }
}
