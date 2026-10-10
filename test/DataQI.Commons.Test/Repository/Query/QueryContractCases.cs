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
            yield return Case("FindByOrderDate",
                "known defect: 'Or' inside 'Order' splits the predicate, legacy throws",
                new DateTime(2020, 1, 1));
            yield return Case("FindByAndroidVersion",
                "known defect: 'And' inside 'Android' splits the predicate, legacy throws",
                "13");
            yield return Case("FindByCategoryInStock",
                "characterization: 'In' is read as the operator, the trailing 'Stock' is ignored",
                (object)new[] { "Tools" });
            yield return Case("FindByNameEquals",
                "legacy has no 'Equals' synonym: parses as the simple property 'NameEquals'",
                "Adams");
        }

        private static object[] Case(string methodName, string description, params object[] arguments)
            => new object[] { new QueryContractCase(methodName, description, arguments) };
    }
}
