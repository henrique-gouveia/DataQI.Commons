using System;
using System.Collections.Generic;
using System.Reflection;

using DataQI.Commons.Query;
using DataQI.Commons.Query.Ast;
using DataQI.Commons.Query.Support;
using DataQI.Commons.Repository.Query;
using DataQI.Commons.Test.Repository.Sample;

using ExpectedObjects;
using Moq;
using Xunit;

namespace DataQI.Commons.Test.Repository.Query
{
    public class QueryMethodParserTest
    {
        private static readonly DateTime Start = new DateTime(2019, 5, 1);
        private static readonly DateTime End = new DateTime(2021, 5, 1);
        private static readonly object[] Cities = { "Fortaleza", "Barcelona", "Manchester" };

        private static ICriterion Single(ICriterion criterion)
            => Restrictions.Disjunction().Add(Restrictions.Conjunction().Add(criterion));

        public static IEnumerable<object[]> CriteriaCases()
        {
            yield return Row("FindByBirthDateBetween", new object[] { Start, End },
                Single(Restrictions.Between("BirthDate", Start, End)));
            yield return Row("FindByHireDateNotBetween", new object[] { Start, End },
                Single(Restrictions.Not(Restrictions.Between("HireDate", Start, End))));
            yield return Row("FindByTitleContaining", new object[] { "Adams" },
                Single(Restrictions.Containing("Title", "Adams")));
            yield return Row("FindByTitleNotContaining", new object[] { "Manager" },
                Single(Restrictions.Not(Restrictions.Containing("Title", "Manager"))));
            yield return Row("FindByTitleEndingWith", new object[] { "IT" },
                Single(Restrictions.EndingWith("Title", "IT")));
            yield return Row("FindByTitleNotEndingWith", new object[] { "Support Agent" },
                Single(Restrictions.Not(Restrictions.EndingWith("Title", "Support Agent"))));
            yield return Row("FindByTitleLike", new object[] { "General" },
                Single(Restrictions.Like("Title", "General")));
            yield return Row("FindByTitleNotLike", new object[] { "Sales" },
                Single(Restrictions.Not(Restrictions.Like("Title", "Sales"))));
            yield return Row("FindByFirstName", new object[] { "Adams" },
                Single(Restrictions.Equal("FirstName", "Adams")));
            // The legacy test expected the property "ByLastName"; it was never compared. The real property is "LastName".
            yield return Row("FindByLastNameNot", new object[] { "Andrew" },
                Single(Restrictions.Not(Restrictions.Equal("LastName", "Andrew"))));
            yield return Row("FindByAgeGreaterThan", new object[] { 30 },
                Single(Restrictions.GreaterThan("Age", 30)));
            yield return Row("FindByAgeGreaterThanEqual", new object[] { 30 },
                Single(Restrictions.GreaterThanEqual("Age", 30)));
            yield return Row("FindByAgeLessThan", new object[] { 30 },
                Single(Restrictions.LessThan("Age", 30)));
            yield return Row("FindByAgeLessThanEqual", new object[] { 30 },
                Single(Restrictions.LessThanEqual("Age", 30)));
            yield return Row("FindByCityIn", new object[] { Cities },
                Single(Restrictions.In("City", Cities)));
            yield return Row("FindByCountryNotIn", new object[] { Cities },
                Single(Restrictions.Not(Restrictions.In("Country", Cities))));
            yield return Row("FindByEmailNull", new object[0],
                Single(Restrictions.Null("Email")));
            yield return Row("FindByPhoneNotNull", new object[0],
                Single(Restrictions.Not(Restrictions.Null("Phone"))));
            yield return Row("FindByFirstNameOrLastName", new object[] { "First", "Last" },
                Restrictions.Disjunction()
                    .Add(Restrictions.Conjunction().Add(Restrictions.Equal("FirstName", "First")))
                    .Add(Restrictions.Conjunction().Add(Restrictions.Equal("LastName", "Last"))));
            yield return Row("FindByFirstNameAndLastName", new object[] { "First", "Last" },
                Restrictions.Disjunction().Add(Restrictions.Conjunction()
                    .Add(Restrictions.Equal("FirstName", "First"))
                    .Add(Restrictions.Equal("LastName", "Last"))));
            yield return Row("FindByStateAndHireDateGreaterThanEqualOrCityInAndEmailEndingWith",
                new object[] { "CE", Start, Cities, "email.com" },
                Restrictions.Disjunction()
                    .Add(Restrictions.Conjunction()
                        .Add(Restrictions.Equal("State", "CE"))
                        .Add(Restrictions.GreaterThanEqual("HireDate", Start)))
                    .Add(Restrictions.Conjunction()
                        .Add(Restrictions.In("City", Cities))
                        .Add(Restrictions.EndingWith("Email", "email.com"))));
            // Property names that merely contain "Or"/"And" are not split (legacy threw here).
            yield return Row("FindByOrderDate", new object[] { Start },
                Single(Restrictions.Equal("OrderDate", Start)));
            yield return Row("FindByAndroidVersion", new object[] { "13" },
                Single(Restrictions.Equal("AndroidVersion", "13")));
            yield return Row("FindByNameAndOrderDate", new object[] { "Ad", Start },
                Restrictions.Disjunction().Add(Restrictions.Conjunction()
                    .Add(Restrictions.Equal("Name", "Ad"))
                    .Add(Restrictions.Equal("OrderDate", Start))));
        }

        private static object[] Row(string methodName, object[] values, ICriterion expected)
            => new object[] { methodName, values, expected };

        [Theory]
        [MemberData(nameof(CriteriaCases))]
        public void TestBuildsTheExpectedPredicate(string methodName, object[] values, ICriterion expected)
        {
            var actual = QueryMethodParser.Parse(methodName).BuildPredicate(values);

            expected.ToExpectedObject().ShouldEqual(actual);
        }

        [Fact]
        public void TestStripsTheAsyncSuffix()
        {
            var actual = QueryMethodParser.Parse("FindByFirstNameAsync").BuildPredicate(new object[] { "Adams" });

            Single(Restrictions.Equal("FirstName", "Adams")).ToExpectedObject().ShouldEqual(actual);
        }

        [Fact]
        public void TestDetectsTheFindByPrefix()
        {
            var actual = QueryMethodParser.Parse("FindByFirstName").BuildPredicate(new object[] { "Adams" });

            Single(Restrictions.Equal("FirstName", "Adams")).ToExpectedObject().ShouldEqual(actual);
        }

        [Theory]
        [InlineData("GetByFirstName")]
        [InlineData("ReadByFirstName")]
        [InlineData("QueryByFirstName")]
        [InlineData("SearchByFirstName")]
        [InlineData("FindOneByFirstName")]
        [InlineData("FindAllByFirstNameAsync")]
        [InlineData("FindBypassByFirstName")]
        [InlineData("GetBypassByFirstName")]
        public void TestDetectsPreviouslyAcceptedQueryPrefixes(string methodName)
        {
            var actual = QueryMethodParser.Parse(methodName).BuildPredicate(new object[] { "Adams" });

            Single(Restrictions.Equal("FirstName", "Adams")).ToExpectedObject().ShouldEqual(actual);
        }

        [Fact]
        public void TestParsesPreviouslyAcceptedReadmeMethodName()
        {
            var actual = QueryMethodParser.Parse("FindFindByFirstNameAndLastNameOrBirthDateGreaterThan")
                .BuildPredicate(new object[] { "First", "Last", Start });
            var expected = Restrictions.Disjunction()
                .Add(Restrictions.Conjunction()
                    .Add(Restrictions.Equal("FirstName", "First"))
                    .Add(Restrictions.Equal("LastName", "Last")))
                .Add(Restrictions.Conjunction()
                    .Add(Restrictions.GreaterThan("BirthDate", Start)));

            expected.ToExpectedObject().ShouldEqual(actual);
        }

        [Theory]
        [InlineData("FindByUpdatedBy", "UpdatedBy")]
        [InlineData("FindByUpdatedByAsync", "UpdatedBy")]
        [InlineData("FindByUpdatedByEqual", "UpdatedBy")]
        [InlineData("FindByUpdatedByName", "UpdatedByName")]
        [InlineData("FindByUpdatedByOrderByFirstName", "UpdatedBy")]
        [InlineData("FindByUpdatedByOrderByFirstNameAsync", "UpdatedBy")]
        [InlineData("GetByUpdatedBy", "UpdatedBy")]
        [InlineData("ReadByUpdatedByName", "UpdatedByName")]
        [InlineData("SearchByUpdatedByOrderByFirstNameAsync", "UpdatedBy")]
        [InlineData("FindFindByUpdatedBy", "UpdatedBy")]
        public void TestPreservesByInsidePropertyName(string methodName, string propertyName)
        {
            var plan = QueryMethodParser.Parse(methodName);
            var actual = plan.BuildPredicate(new object[] { "Adams" });

            Single(Restrictions.Equal(propertyName, "Adams")).ToExpectedObject().ShouldEqual(actual);
        }

        [Theory]
        [MemberData(nameof(NonStringTextMatchCases))]
        public void TestTextMatchRejectsNonStringValueClearly(string methodName, object value)
        {
            var plan = QueryMethodParser.Parse(methodName);

            var exception = Assert.Throws<ArgumentException>(() =>
                plan.BuildPredicate(new object[] { value }));

            Assert.Equal("values", exception.ParamName);
            Assert.StartsWith("Value for text criterion on property 'Title' must be a string.", exception.Message);
        }

        public static IEnumerable<object[]> NonStringTextMatchCases()
        {
            var methods = new[]
            {
                "FindByTitleContaining", "FindByTitleEndingWith", "FindByTitleLike", "FindByTitleStartingWith"
            };
            var values = new object[] { 123, true, 1.5m, new[] { "Adams" } };

            foreach (var method in methods)
                foreach (var value in values)
                    yield return new object[] { method, value };
        }

        [Theory]
        [InlineData("FindByTitleContaining", TextMatchKind.Containing, "Ad")]
        [InlineData("FindByTitleContaining", TextMatchKind.Containing, null)]
        [InlineData("FindByTitleEndingWith", TextMatchKind.EndingWith, "Ad")]
        [InlineData("FindByTitleEndingWith", TextMatchKind.EndingWith, null)]
        [InlineData("FindByTitleLike", TextMatchKind.Like, "Ad")]
        [InlineData("FindByTitleLike", TextMatchKind.Like, null)]
        [InlineData("FindByTitleStartingWith", TextMatchKind.StartingWith, "Ad")]
        [InlineData("FindByTitleStartingWith", TextMatchKind.StartingWith, null)]
        public void TestTextMatchPreservesStringAndNullValues(string methodName, TextMatchKind kind, string value)
        {
            var actual = QueryMethodParser.Parse(methodName).BuildPredicate(new object[] { value });

            Single(new TextMatch("Title", kind, value)).ToExpectedObject().ShouldEqual(actual);
        }

        [Theory]
        [InlineData("Containing", TextMatchKind.Containing)]
        [InlineData("EndingWith", TextMatchKind.EndingWith)]
        [InlineData("Like", TextMatchKind.Like)]
        [InlineData("StartingWith", TextMatchKind.StartingWith)]
        public void TestTextMatchConsumesOnlyItsOwnValue(string keyword, TextMatchKind kind)
        {
            var actual = QueryMethodParser.Parse($"FindByFirstNameAndTitle{keyword}AndAge")
                .BuildPredicate(new object[] { "Adams", "Manager", 30 });
            var expected = Restrictions.Disjunction().Add(Restrictions.Conjunction()
                .Add(Restrictions.Equal("FirstName", "Adams"))
                .Add(new TextMatch("Title", kind, "Manager"))
                .Add(Restrictions.Equal("Age", 30)));

            expected.ToExpectedObject().ShouldEqual(actual);
        }

        [Theory]
        [InlineData("FindByTitleContaining")]
        [InlineData("FindByTitleEndingWith")]
        [InlineData("FindByTitleLike")]
        [InlineData("FindByTitleStartingWith")]
        public void TestTextMatchRejectsMissingValue(string methodName)
        {
            var plan = QueryMethodParser.Parse(methodName);

            var exception = Assert.Throws<ArgumentException>(() => plan.BuildPredicate(new object[0]));

            Assert.Equal("The query method needs more values than were supplied", exception.Message);
        }

        [Fact]
        public void TestParsesANameWithoutAPrefixAsASimpleProperty()
        {
            var actual = QueryMethodParser.Parse("FirstName").BuildPredicate(new object[] { "Adams" });

            Single(Restrictions.Equal("FirstName", "Adams")).ToExpectedObject().ShouldEqual(actual);
        }

        [Fact]
        public void TestRejectsNullMethod()
        {
            var exception = Assert.Throws<ArgumentException>(() => QueryMethodParser.Parse((MethodInfo)null));

            Assert.Equal("Query Method must not be null", exception.GetBaseException().Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("FindBy")]
        [InlineData("GetBy")]
        [InlineData("ReadBy")]
        [InlineData("SearchBy")]
        public void TestRejectsAnEmptyPredicate(string methodName)
        {
            var exception = Assert.Throws<ArgumentException>(() => QueryMethodParser.Parse(methodName));

            Assert.Equal("Source must not be null or empty", exception.GetBaseException().Message);
        }

        [Fact]
        public void TestParsesAMethodInfoThroughItsName()
        {
            MethodInfo method = new Mock<IFakeRepository>().Object.GetType().GetMethod("FindByFirstName");

            var actual = QueryMethodParser.Parse(method).BuildPredicate(new object[] { "Adams" });

            Single(Restrictions.Equal("FirstName", "Adams")).ToExpectedObject().ShouldEqual(actual);
        }

        [Fact]
        public void TestRejectsTooFewValues()
        {
            var plan = QueryMethodParser.Parse("FindByBirthDateBetween");

            var exception = Assert.Throws<ArgumentException>(() => plan.BuildPredicate(new object[] { Start }));

            Assert.Contains("more values", exception.GetBaseException().Message);
        }
    }
}
