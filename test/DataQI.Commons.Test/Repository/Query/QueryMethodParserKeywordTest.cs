using System;

using DataQI.Commons.Query.Ast;
using DataQI.Commons.Repository.Query;

using ExpectedObjects;
using Xunit;

namespace DataQI.Commons.Test.Repository.Query
{
    public class QueryMethodParserKeywordTest
    {
        [Theory]
        [InlineData("firstName", "firstName", "SimpleProperty", false)]
        [InlineData("DateOfBirthBetween", "DateOfBirth", "Between", false)]
        [InlineData("DateOfBirthIsBetween", "DateOfBirth", "Between", false)]
        [InlineData("DateOfBirthNotBetween", "DateOfBirth", "Between", true)]
        [InlineData("DateOfBirthIsNotBetween", "DateOfBirth", "Between", true)]
        [InlineData("LastNameContaining", "LastName", "Containing", false)]
        [InlineData("LastNameIsContaining", "LastName", "Containing", false)]
        [InlineData("LastNameContains", "LastName", "Containing", false)]
        [InlineData("LastNameIsNotContaining", "LastName", "Containing", true)]
        [InlineData("LastNameNotContaining", "LastName", "Containing", true)]
        [InlineData("LastNameNotContains", "LastName", "Containing", true)]
        [InlineData("LastNameIsEndingWith", "LastName", "EndingWith", false)]
        [InlineData("LastNameEndingWith", "LastName", "EndingWith", false)]
        [InlineData("LastNameEndsWith", "LastName", "EndingWith", false)]
        [InlineData("LastNameIsNotEndingWith", "LastName", "EndingWith", true)]
        [InlineData("LastNameNotEndingWith", "LastName", "EndingWith", true)]
        [InlineData("LastNameNotEndsWith", "LastName", "EndingWith", true)]
        [InlineData("firstNameIsEqual", "firstName", "Equal", false)]
        [InlineData("firstNameEqual", "firstName", "Equal", false)]
        [InlineData("NameEquals", "Name", "Equal", false)]
        [InlineData("NameIsEquals", "Name", "Equal", false)]
        [InlineData("AgeIsNotEqual", "Age", "Equal", true)]
        [InlineData("AgeNotEqual", "Age", "Equal", true)]
        [InlineData("DateOfBirthIsGreaterThan", "DateOfBirth", "GreaterThan", false)]
        [InlineData("DateOfBirthGreaterThan", "DateOfBirth", "GreaterThan", false)]
        [InlineData("DateOfBirthIsGreaterThanEqual", "DateOfBirth", "GreaterThanEqual", false)]
        [InlineData("DateOfBirthGreaterThanEqual", "DateOfBirth", "GreaterThanEqual", false)]
        [InlineData("InvoiceIdIn", "InvoiceId", "In", false)]
        [InlineData("InvoiceIdIsIn", "InvoiceId", "In", false)]
        [InlineData("InvoiceIdIsNotIn", "InvoiceId", "In", true)]
        [InlineData("InvoiceIdNotIn", "InvoiceId", "In", true)]
        [InlineData("PhoneIsNull", "Phone", "Null", false)]
        [InlineData("PhoneNull", "Phone", "Null", false)]
        [InlineData("PhoneIsNotNull", "Phone", "Null", true)]
        [InlineData("PhoneNotNull", "Phone", "Null", true)]
        [InlineData("DateOfBirthIsLessThan", "DateOfBirth", "LessThan", false)]
        [InlineData("DateOfBirthLessThan", "DateOfBirth", "LessThan", false)]
        [InlineData("DateOfBirthIsLessThanEqual", "DateOfBirth", "LessThanEqual", false)]
        [InlineData("DateOfBirthLessThanEqual", "DateOfBirth", "LessThanEqual", false)]
        [InlineData("LastNameIsLike", "LastName", "Like", false)]
        [InlineData("LastNameLike", "LastName", "Like", false)]
        [InlineData("LastNameIsNotLike", "LastName", "Like", true)]
        [InlineData("LastNameNotLike", "LastName", "Like", true)]
        [InlineData("LastNameIsStartingWith", "LastName", "StartingWith", false)]
        [InlineData("LastNameStartingWith", "LastName", "StartingWith", false)]
        [InlineData("LastNameStartsWith", "LastName", "StartingWith", false)]
        [InlineData("LastNameIsNotStartingWith", "LastName", "StartingWith", true)]
        [InlineData("LastNameNotStartingWith", "LastName", "StartingWith", true)]
        [InlineData("LastNameNotStartsWith", "LastName", "StartingWith", true)]
        public void TestParsesTheKeywordVocabulary(string source, string property, string kind, bool negated)
        {
            var values = ValuesFor(kind);

            var actual = QueryMethodParser.Parse($"FindBy{source}").BuildPredicate(values);

            var expected = ExpectedFor(kind, property, values);
            if (negated)
                expected = Restrictions.Not(expected);

            Restrictions.Disjunction()
                .Add(Restrictions.Conjunction().Add(expected))
                .ToExpectedObject().ShouldEqual(actual);
        }

        private static object[] ValuesFor(string kind)
        {
            switch (kind)
            {
                case "Between": return new object[] { 1, 2 };
                case "In": return new object[] { new object[] { "a", "b" } };
                case "Null": return new object[0];
                default: return new object[] { "v" };
            }
        }

        private static ICriterion ExpectedFor(string kind, string property, object[] values)
        {
            switch (kind)
            {
                case "Between": return Restrictions.Between(property, values[0], values[1]);
                case "In": return Restrictions.In(property, (object[])values[0]);
                case "Null": return Restrictions.Null(property);
                case "Containing": return Restrictions.Containing(property, "v");
                case "EndingWith": return Restrictions.EndingWith(property, "v");
                case "Like": return Restrictions.Like(property, "v");
                case "StartingWith": return Restrictions.StartingWith(property, "v");
                case "GreaterThan": return Restrictions.GreaterThan(property, "v");
                case "GreaterThanEqual": return Restrictions.GreaterThanEqual(property, "v");
                case "LessThan": return Restrictions.LessThan(property, "v");
                case "LessThanEqual": return Restrictions.LessThanEqual(property, "v");
                case "Equal":
                case "SimpleProperty": return Restrictions.Equal(property, "v");
                default: throw new ArgumentException($"Unknown kind {kind}");
            }
        }
    }
}
