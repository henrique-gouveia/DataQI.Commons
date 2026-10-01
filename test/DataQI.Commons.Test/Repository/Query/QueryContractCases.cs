using System;
using System.Collections.Generic;

namespace DataQI.Commons.Test.Repository.Query
{
    public static class QueryContractCases
    {
        public static IEnumerable<object[]> All()
        {
            yield return Case("FindByNameStartingWithAndStockGreaterThanOrDepartmentIn",
                "Name (StartingWith) AND Stock (GreaterThan) OR Department (In)",
                "Ad", 10m, new[] { "Sales", "Support" });
            yield return Case("FindByNameNotLike", "Name, Not + Like",
                "%Ad%");
            yield return Case("FindByNameIsNotNull", "Name, Not + Null");
            yield return Throwing("FindByOrderDate",
                "known defect: 'Or' inside 'Order' splits the predicate, legacy throws",
                new DateTime(2020, 1, 1));
            yield return Throwing("FindByAndroidVersion",
                "known defect: 'And' inside 'Android' splits the predicate, legacy throws",
                "13");
            yield return Case("FindByCategoryInStock",
                "characterization: 'In' is read as the operator, the trailing 'Stock' is ignored",
                (object)new[] { "Tools" });
            yield return Case("FindByNameEquals",
                "legacy has no 'Equals' synonym: parses as the simple property 'NameEquals'",
                "Adams");
        }

        // A lone string[] argument must be cast to object at the call site: otherwise C# treats it as the
        // params array itself and the method receives "Tools" instead of { string[] }.
        private static object[] Case(string methodName, string description, params object[] arguments)
            => new object[] { new QueryContractCase(methodName, description, arguments) };

        private static object[] Throwing(string methodName, string description, params object[] arguments)
            => new object[]
            {
                new QueryContractCase(methodName, description, arguments,
                    expectsException: true, expectedExceptionMessage: "Source must not be null or empty")
            };
    }
}
