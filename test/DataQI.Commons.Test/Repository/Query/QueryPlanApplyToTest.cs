using System;
using System.Linq;

using DataQI.Commons.Query;
using DataQI.Commons.Query.Support;
using DataQI.Commons.Repository.Query;

using Xunit;
using Ast = DataQI.Commons.Query.Ast;

namespace DataQI.Commons.Test.Repository.Query
{
    public class QueryPlanApplyToTest
    {
        [Fact]
        public void TestApplyToRejectsNullCriteria()
        {
            var plan = QueryMethodParser.Parse("FindByFirstName");

            var exception = Assert.Throws<ArgumentException>(() => plan.ApplyTo(null, new object[] { "Adams" }));

            Assert.Equal("Criteria must be not null", exception.GetBaseException().Message);
        }

        [Fact]
        public void TestApplyToAddsThePredicateAndTheOrdersInSequence()
        {
            var plan = QueryMethodParser.Parse("FindByFirstNameOrderByLastNameAscBirthDateDesc");
            var criteria = new Criteria();

            plan.ApplyTo(criteria, new object[] { "Adams" });

            var predicate = Assert.Single(criteria.Criterions);
            Assert.IsType<Ast.Junction>(predicate);
            Assert.Equal(2, criteria.Orders.Count);
            Assert.Equal("LastName", criteria.Orders.ElementAt(0).GetPropertyName());
            Assert.Equal(OrderDirection.Desc, criteria.Orders.ElementAt(1).GetDirection());
        }
    }
}
