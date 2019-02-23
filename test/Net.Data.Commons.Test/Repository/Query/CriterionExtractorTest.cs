using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;
using ExpectedObjects;

using Net.Data.Commons.Repository.Query;

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

        private void AssertCriterionsIn(CriterionExtractor.OrCriterion criterion, Criterion[] exptectedCriterions) 
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
            AssertSuportCriterionType("firstName", "firstName", CriterionType.SimpleProperty);
        }

        [Fact]
        public void TestSupportBeteweenTypeCorrectly()
        {
            AssertSuportCriterionType("DateOfBirthBetween", "DateOfBirth", CriterionType.Between);
        }

        [Fact]
        public void TestSupportNotBeteweenTypeCorrectly()
        {
            AssertSuportCriterionType("DateOfBirthNotBetween", "DateOfBirth", CriterionType.NotBetween);
        }

        [Fact]
        public void TestSupportContainingTypeCorrectly()
        {
            AssertSuportCriterionType("LastNameContaining", "LastName", CriterionType.Containing);
        }

        [Fact]
        public void TestSupportNotContainingTypeCorrectly()
        {
            AssertSuportCriterionType("LastNameNotContaining", "LastName", CriterionType.NotContaining);
        }

        [Fact]
        public void TestSupportEqualsTypeCorrectly()
        {
            AssertSuportCriterionType("firstNameEquals", "firstName", CriterionType.Equals);
        }

        [Fact]
        public void TestSupportNotEqualsTypeCorrectly()
        {
            AssertSuportCriterionType("AgeNotEquals", "Age", CriterionType.NotEquals);
        }

        [Fact]
        public void TestSupportInTypeCorrectly()
        {
            AssertSuportCriterionType("AgeIn", "Age", CriterionType.In);
        }

        [Fact]
        public void TestSupportNotInTypeCorrectly()
        {
            AssertSuportCriterionType("AgeNotIn", "Age", CriterionType.NotIn);
        }

        [Fact]
        public void TestSupportIsNullTypeCorrectly()
        {
            AssertSuportCriterionType("PhoneIsNull", "Phone", CriterionType.IsNull);
        }

        [Fact]
        public void TestSupportIsNotNullTypeCorrectly()
        {
            AssertSuportCriterionType("PhoneIsNotNull", "Phone", CriterionType.IsNotNull);
        }

        [Fact]
        public void TestSupportLessThanTypeCorrectly()
        {
            AssertSuportCriterionType("DateOfBirthLessThan", "DateOfBirth", CriterionType.LessThan);
        }

        [Fact]
        public void TestSupportLessThanEqualTypeCorrectly()
        {
            AssertSuportCriterionType("DateOfBirthLessThanEqual", "DateOfBirth", CriterionType.LessThanEqual);
        }

        [Fact]
        public void TestSupportGreatherThanTypeCorrectly()
        {
            AssertSuportCriterionType("DateOfBirthGreatherThan", "DateOfBirth", CriterionType.GreatherThan);
        }

        [Fact]
        public void TestSupportGreatherThanEqualTypeCorrectly()
        {
            AssertSuportCriterionType("DateOfBirthGreatherThanEqual", "DateOfBirth", CriterionType.GreatherThanEqual);
        }

        [Fact]
        public void TestSupportEndingWithTypeCorrectly()
        {
            AssertSuportCriterionType("LastNameEndingWith", "LastName", CriterionType.EndingWith);
        }

        [Fact]
        public void TestSupportNotEndingWithTypeCorrectly()
        {
            AssertSuportCriterionType("LastNameNotEndingWith", "LastName", CriterionType.NotEndingWith);
        }

        [Fact]
        public void TestSupportStartingWithTypeCorrectly()
        {
            AssertSuportCriterionType("LastNameStartingWith", "LastName", CriterionType.StartingWith);
        }

        [Fact]
        public void TestSupportNotStartingWithTypeCorrectly()
        {
            AssertSuportCriterionType("LastNameNotStartingWith", "LastName", CriterionType.NotStartingWith);
        }

        private void AssertSuportCriterionType(string source, string propertyNameExptected, CriterionType criterionTypeExpected)
        {
            var criterion = new Criterion(source);
            Assert.Equal(criterionTypeExpected, criterion.Type);
            Assert.Equal(propertyNameExptected, criterion.PropertyName);
        }
    }
}
