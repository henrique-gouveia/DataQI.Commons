using System;


using DataQI.Commons.Query;
using DataQI.Commons.Query.Ast;
using DataQI.Commons.Query.Support;
namespace DataQI.Commons.Test.Repository.Query
{
    public static class QueryRedesignedContractExpectations
    {
        private static ICriterion Single(ICriterion criterion)
            => Restrictions.Disjunction().Add(Restrictions.Conjunction().Add(criterion));

        public static ICriterion For(string methodName)
        {
            switch (methodName)
            {
                case "FindByNameStartingWithAndStockGreaterThanOrDepartmentIn":
                    return Restrictions.Disjunction()
                        .Add(Restrictions.Conjunction()
                            .Add(Restrictions.StartingWith("Name", "Ad"))
                            .Add(Restrictions.GreaterThan("Stock", 10m)))
                        .Add(Restrictions.Conjunction()
                            .Add(Restrictions.In("Department", new object[] { "Sales", "Support" })));
                case "FindByNameNotLike":
                    return Single(Restrictions.Not(Restrictions.Like("Name", "%Ad%")));
                case "FindByNameIsNotNull":
                    return Single(Restrictions.Not(Restrictions.Null("Name")));
                case "FindByOrderDate":
                    return Single(Restrictions.Equal("OrderDate", new DateTime(2020, 1, 1)));
                case "FindByAndroidVersion":
                    return Single(Restrictions.Equal("AndroidVersion", "13"));
                case "FindByCategoryInStock":
                    // A stricter parser could recognize operators only at the end of a member,
                    // interpreting "CategoryInStock" as a property with an equality comparison.
                    return Single(Restrictions.In("Category", new object[] { "Tools" }));
                case "FindByNameEquals":
                    return Single(Restrictions.Equal("Name", "Adams"));
                default:
                    throw new ArgumentException($"No redesigned expectation for {methodName}");
            }
        }
    }
}
