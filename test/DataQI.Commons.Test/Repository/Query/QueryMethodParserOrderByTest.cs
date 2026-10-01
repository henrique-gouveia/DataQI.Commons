using System.Collections.Generic;
using System.Linq;

using DataQI.Commons.Query.Support;
using DataQI.Commons.Repository.Query;

using Xunit;

namespace DataQI.Commons.Test.Repository.Query
{
    public class QueryMethodParserOrderByTest
    {
        [Theory]
        [InlineData("FindByActiveOrderByName", "Name:Asc")]
        [InlineData("FindByActiveOrderByNameDesc", "Name:Desc")]
        [InlineData("FindByActiveOrderByNameAscStockDesc", "Name:Asc,Stock:Desc")]
        [InlineData("FindByActiveOrderByNameAscStock", "Name:Asc,Stock:Asc")]
        [InlineData("FindByFirstNameOrderByLastNameAscBirthDateDesc", "LastName:Asc,BirthDate:Desc")]
        // Only the trailing property may omit its direction: without a marker between them the two
        // properties cannot be told apart, so this reads as ONE property called NameStock.
        [InlineData("FindByActiveOrderByNameStockDesc", "NameStock:Desc")]
        // A direction word followed by a digit/lowercase letter is part of the property name.
        [InlineData("FindByActiveOrderByNameAsc2Factor", "NameAsc2Factor:Asc")]
        [InlineData("FindByActiveOrderByNameDescAsync", "Name:Desc")]
        [InlineData("FindByActive", "")]
        public void TestParsesTheOrderingClause(string methodName, string expected)
        {
            var plan = QueryMethodParser.Parse(methodName);

            var actual = string.Join(",", plan.Orders.Select(o => $"{o.GetPropertyName()}:{o.GetDirection()}"));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestOrderByDoesNotLeakIntoThePredicate()
        {
            var plan = QueryMethodParser.Parse("FindByActiveOrderByNameDesc");

            var predicate = plan.BuildPredicate(new object[] { true });

            var or = Assert.IsType<DataQI.Commons.Query.Ast.Junction>(predicate);
            var and = Assert.IsType<DataQI.Commons.Query.Ast.Junction>(Assert.Single(or.Members));
            var comparison = Assert.IsType<DataQI.Commons.Query.Ast.Comparison>(Assert.Single(and.Members));
            Assert.Equal("Active", comparison.PropertyName);
        }

        [Fact]
        public void TestOrdersAreInCallSequence()
        {
            var plan = QueryMethodParser.Parse("FindByActiveOrderByNameAscStockDesc");

            Assert.Equal(OrderDirection.Asc, plan.Orders[0].GetDirection());
            Assert.Equal(OrderDirection.Desc, plan.Orders[1].GetDirection());
        }
    }
}
