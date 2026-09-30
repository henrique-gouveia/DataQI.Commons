using System.Collections.Generic;

namespace DataQI.Commons.Test.Repository.Query
{
    public static class QueryContractCases
    {
        public static IEnumerable<object[]> All()
        {
            yield return Case("FindByNameStartingWithAndStockGreaterThanOrDepartmentIn",
                "Name (StartingWith) AND Stock (GreaterThan) OR Department (In)");
            yield return Case("FindByNameNotLike", "Name, Not + Like");
            yield return Case("FindByNameIsNotNull", "Name, Not + Null");
            yield return Case("FindByOrderDate", "known defect: throws today, fixed by this plan",
                expectsException: true, expectedExceptionMessage: "Source must not be null or empty");
            yield return Case("FindByAndroidVersion", "known defect: throws today, fixed by this plan",
                expectsException: true, expectedExceptionMessage: "Source must not be null or empty");
            yield return Case("FindByCategoryInStock",
                "known defect: Category (In) parses, Stock silently discarded today, fixed by this plan");
            yield return Case("FindByNameEquals", "Name via Equals synonym, fixed by this plan");
        }

        private static object[] Case(string methodName, string description, bool expectsException = false, string expectedExceptionMessage = null)
            => new object[] { new QueryContractCase(methodName, description, expectsException, expectedExceptionMessage) };
    }
}
