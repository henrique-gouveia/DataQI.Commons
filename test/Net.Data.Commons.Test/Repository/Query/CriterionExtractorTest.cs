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

        [Fact]
        public void TestSupportSimplePropertyCorrectly()
        {
            AssertSuportCriterionType(CriterionType.SimpleProperty, "firstName", "firstName");
        }

        [Fact]
        public void TestSupportFirstNameEqualsCorrectly()
        {
            AssertSuportCriterionType(CriterionType.Equals, "firstName", "firstNameEquals");
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

        private void AssertSuportCriterionType(CriterionType criterionTypeExpected, string propertyName, params string[] sources)
        {
            foreach (var source in sources)
            {
                var criterion = new Criterion(source);
                Assert.Equal(criterionTypeExpected, criterion.Data.Type);
                Assert.Equal(propertyName, criterion.PropertyName);
            }
        }
    }
}