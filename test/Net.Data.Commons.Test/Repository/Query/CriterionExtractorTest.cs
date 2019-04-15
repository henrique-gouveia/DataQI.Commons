using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;
using ExpectedObjects;

using Net.Data.Commons.Repository.Query;
using Net.Data.Commons.Criterions.Support;

using static Net.Data.Commons.Repository.Query.CriterionExtractor;

namespace Net.Data.Commons.Test.Repository.Query
{
    public class CriterionExtractorTest
    {
        [Fact]
        public void TestRejectsNullSource() 
        {
            Assert.Throws<ArgumentException>(() => new CriterionExtractor(null));
            Assert.Throws<ArgumentException>(() => new CriterionExtractor(""));
        }

        [Fact]
        public void TestExtractSimplePropertyCorrectly()
        {
            var extractor = new CriterionExtractor("FirstName");
            AssertExtractor(extractor, Criterions("FirstName"));
        }

        [Fact]
        public void TestExtractAndPropertiesCorrectly()
        {
            var extractor = new CriterionExtractor("FirstNameAndLastName");
            AssertExtractor(extractor, Criterions("FirstName", "LastName"));
        }

        [Fact]
        public void TestExtractOrPropertiesCorrectly()
        {
            var extractor = new CriterionExtractor("FirstNameOrLastName");
            AssertExtractor(extractor, Criterions("FirstName" ), Criterions("LastName"));
        }

        [Fact]
        public void TestExtractCombinedAndAndOrAndAndPropertiesCorrectly()
        {
            var extractor = new CriterionExtractor("FirstNameAndLastNameOrAgeAndEmail");
            AssertExtractor(extractor, Criterions("FirstName", "LastName"), Criterions("Age", "Email"));
        }

        [Fact]
        public void TestDetectsPrefixCorrectly()
        {
            var extractor = new CriterionExtractor("FindByFirstName");
            AssertExtractor(extractor, Criterions("FirstName"));
        }

        private Criterion[] Criterions(params string[] criterion)
        {
            var criterions = new List<Criterion>();
            foreach(var item in criterion)
                criterions.Add(Criterion(item));

            return criterions.ToArray();
        }

        private Criterion Criterion(string criterion)
        {
            return new Criterion(criterion);
        }
        
        private void AssertExtractor(CriterionExtractor extractor, params Criterion[][] exptectedCriterions)
        {
            var criterions = extractor.GetEnumerator();

            foreach (var expectedCriterion in exptectedCriterions)
            {
               Assert.True(criterions.MoveNext());
               AssertCriterionsIn(criterions.Current, expectedCriterion);
            }

            Assert.False(criterions.MoveNext());
        }

        private void AssertCriterionsIn(OrCriterion criterion, Criterion[] exptectedCriterions) 
        {
            var criterions = criterion.GetEnumerator();

            foreach (var expectedCriterion in exptectedCriterions)
            {
                Assert.True(criterions.MoveNext());
                expectedCriterion.ToExpectedObject().ShouldMatch(criterions.Current);
            }

            Assert.False(criterions.MoveNext());
        }

        [Fact]
        public void TestRejectsCriterionNullSource() 
        {
            Assert.Throws<ArgumentException>(() => new Criterion(null));
            Assert.Throws<ArgumentException>(() => new Criterion(""));
        }
        
        [Fact]
        public void TestSupportSimplePropertyTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "firstName", 
                propertyNameExptected: "firstName", 
                criterionTypeExpected: CriterionType.SimpleProperty,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} = {1}");
        }

        [Fact]
        public void TestSupportBeteweenTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "DateOfBirthBetween", 
                propertyNameExptected: "DateOfBirth", 
                criterionTypeExpected: CriterionType.Between,
                numberOfArgsExpected: 2,
                commandTemplateExpected: "{0} BETWEEN {1} AND {2}");
        }

        [Fact]
        public void TestSupportNotBeteweenTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "DateOfBirthNotBetween", 
                propertyNameExptected: "DateOfBirth", 
                criterionTypeExpected: CriterionType.NotBetween,
                numberOfArgsExpected: 2,
                commandTemplateExpected: "{0} NOT BETWEEN {1} AND {2}");
        }

        [Fact]
        public void TestSupportContainingTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "LastNameContaining", 
                propertyNameExptected: "LastName", 
                criterionTypeExpected: CriterionType.Containing,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} LIKE %{1}%");
        }

        [Fact]
        public void TestSupportNotContainingTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "LastNameNotContaining", 
                propertyNameExptected: "LastName", 
                criterionTypeExpected: CriterionType.NotContaining,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} NOT LIKE %{1}%");
        }

        [Fact]
        public void TestSupportEndingWithTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "LastNameEndingWith", 
                propertyNameExptected: "LastName", 
                criterionTypeExpected: CriterionType.EndingWith,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} LIKE %{1}");
        }

        [Fact]
        public void TestSupportNotEndingWithTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "LastNameNotEndingWith", 
                propertyNameExptected: "LastName", 
                criterionTypeExpected: CriterionType.NotEndingWith,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} NOT LIKE %{1}");
        }

        [Fact]
        public void TestSupportEqualsTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "firstNameEquals", 
                propertyNameExptected: "firstName", 
                criterionTypeExpected: CriterionType.Equals,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} = {1}");
        }
        
        [Fact]
        public void TestSupportNotEqualsTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "AgeNotEquals", 
                propertyNameExptected: "Age", 
                criterionTypeExpected: CriterionType.NotEquals,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} <> {1}");
        }

        [Fact]
        public void TestSupportGreatherThanTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "DateOfBirthGreatherThan", 
                propertyNameExptected: "DateOfBirth", 
                criterionTypeExpected: CriterionType.GreatherThan,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} > {1}");
        }

        [Fact]
        public void TestSupportGreatherThanEqualTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "DateOfBirthGreatherThanEqual", 
                propertyNameExptected: "DateOfBirth", 
                criterionTypeExpected: CriterionType.GreatherThanEqual,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} >= {1}");
        }

        [Fact]
        public void TestSupportInTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "AgeIn", 
                propertyNameExptected: "Age", 
                criterionTypeExpected: CriterionType.In,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} IN {1}");
        }

        [Fact]
        public void TestSupportNotInTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "AgeNotIn", 
                propertyNameExptected: "Age", 
                criterionTypeExpected: CriterionType.NotIn,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} NOT IN {1}");
        }

        [Fact]
        public void TestSupportIsNullTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "PhoneIsNull", 
                propertyNameExptected: "Phone", 
                criterionTypeExpected: CriterionType.IsNull,
                numberOfArgsExpected: 0,
                commandTemplateExpected: "{0} IS NULL");
        }

        [Fact]
        public void TestSupportIsNotNullTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "PhoneIsNotNull", 
                propertyNameExptected: "Phone", 
                criterionTypeExpected: CriterionType.IsNotNull,
                numberOfArgsExpected: 0,
                commandTemplateExpected: "{0} IS NOT NULL");
        }

        [Fact]
        public void TestSupportLessThanTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "DateOfBirthLessThan", 
                propertyNameExptected: "DateOfBirth", 
                criterionTypeExpected: CriterionType.LessThan,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} < {1}");
        }

        [Fact]
        public void TestSupportLessThanEqualTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "DateOfBirthLessThanEqual", 
                propertyNameExptected: "DateOfBirth", 
                criterionTypeExpected: CriterionType.LessThanEqual,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} <= {1}");
        }

        [Fact]
        public void TestSupportStartingWithTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "LastNameStartingWith", 
                propertyNameExptected: "LastName", 
                criterionTypeExpected: CriterionType.StartingWith,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} LIKE {1}%");
        }

        [Fact]
        public void TestSupportNotStartingWithTypeCorrectly()
        {
            AssertSuportCriterionType(
                source: "LastNameNotStartingWith", 
                propertyNameExptected: "LastName", 
                criterionTypeExpected: CriterionType.NotStartingWith,
                numberOfArgsExpected: 1,
                commandTemplateExpected: "{0} NOT LIKE {1}%");
        }

        private void AssertSuportCriterionType(string source, string propertyNameExptected, CriterionType criterionTypeExpected, 
            int numberOfArgsExpected, string commandTemplateExpected)
        {
            var criterion = new Criterion(source);
            Assert.Equal(propertyNameExptected, criterion.PropertyName);
            Assert.Equal(criterionTypeExpected, criterion.Type);
            Assert.Equal(numberOfArgsExpected, criterion.Type.NumberOfArgs());
            Assert.Equal(commandTemplateExpected, criterion.Type.CommandTemplate());
        }
    }
}
