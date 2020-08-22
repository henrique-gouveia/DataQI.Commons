using System;
using DataQI.Commons.Query;
using DataQI.Commons.Query.Support;

using Xunit;

namespace DataQI.Commons.Test.Query
{
    public class RestrictionsTest
    {
        [Fact]
        public void TestCreateBetweenCorrectly()
        {
            var propertyName = "Age";
            var values = new object[] {10, 20};
            
            ICriterion criterion = Restrictions.Between(propertyName, values[0], values[1]);
            
            AssertCriterion(criterion, propertyName, WhereOperator.Between, values);
        }

        [Fact]
        public void TestCreateContaingCorrectly()
        {
            var propertyName = "Age";
            var value = "%name%";
            
            ICriterion criterion = Restrictions.Containing(propertyName, value);

            AssertCriterion(criterion, propertyName, WhereOperator.Containing, value);
        }

        [Fact]
        public void TestCreateEndingWithCorrectly()
        {
            var propertyName = "Name";
            var value = "%name";

            ICriterion criterion = Restrictions.EndingWith("Name", value);

            AssertCriterion(criterion, propertyName, WhereOperator.EndingWith, value);
        }

        [Fact]
        public void TestCreateGreaterThanCorrectly()
        {
            var propertyName = "DateOfBirth";
            var value = DateTime.Now;

            ICriterion criterion = Restrictions.GreaterThan(propertyName, value);

            AssertCriterion(criterion, propertyName, WhereOperator.GreaterThan, value);
        }

        [Fact]
        public void TestCreateGreaterThanEqualCorrectly()
        {
            var propertyName = "DateOfBirth";
            var value = DateTime.Now;

            ICriterion criterion = Restrictions.GreaterThanEqual(propertyName, value);

            AssertCriterion(criterion, propertyName, WhereOperator.GreaterThanEqual, value);
        }
        
        [Fact]
        public void TestCreateEqualCorrectly()
        {
            var propertyName = "Name";
            var value = "Fake";

            ICriterion criterion = Restrictions.Equal(propertyName, value);

            AssertCriterion(criterion, propertyName, WhereOperator.Equal, value);
        }
        
        [Fact]
        public void TestCreateInCorrectly()
        {
            var propertyName = "Departments";
            var values = new string[] { "Shirt", "Shoes" };

            ICriterion criterion = Restrictions.In(propertyName, values);

            AssertCriterion(criterion, propertyName, WhereOperator.In, values);
        }

        [Fact]
        public void TestCreateLessThanCorrectly()
        {
            var propertyName = "DateOfBirth";
            var value = DateTime.Now;

            ICriterion criterion = Restrictions.LessThan(propertyName, value);

            AssertCriterion(criterion, propertyName, WhereOperator.LessThan, value);
        }

        [Fact]
        public void TestCreateLessThanEqualCorrectly()
        {
            var propertyName = "DateOfBirth";
            var value = DateTime.Now;

            ICriterion criterion = Restrictions.LessThanEqual(propertyName, value);

            AssertCriterion(criterion, propertyName, WhereOperator.LessThanEqual, value);
        }

        [Fact]
        public void TestCreateLikeCorrectly()
        {
            var propertyName = "Email";
            var value = "fake@%";

            ICriterion criterion = Restrictions.Like(propertyName, value);

            AssertCriterion(criterion, propertyName, WhereOperator.Like, value);
        }

        [Fact]
        public void TestCreateNotCorrectly()
        {
            var propertyName = "Email";
            var value = "fake@fake.com";

            ICriterion criterion = Restrictions.Not(Restrictions.Equal(propertyName, value));

            AssertCriterion(criterion, propertyName, WhereOperator.Not, Array.Empty<object>());
        }

        [Fact]
        public void TestCreateNullCorrectly()
        {
            var propertyName = "Email";

            ICriterion criterion = Restrictions.Null(propertyName);

            AssertCriterion(criterion, propertyName, WhereOperator.Null, Array.Empty<object>());
        }

        [Fact]
        public void TestCreateStartingWithCorrectly()
        {
            var propertyName = "Name";
            var value = "name%";

            ICriterion criterion = Restrictions.StartingWith("Name", value);

            AssertCriterion(criterion, propertyName, WhereOperator.StartingWith, value);
        }        

        [Fact]
        public void TestCreateConjuctionCorrectly()
        {
            IJunction junction = Restrictions.Conjunction();
            
            Assert.IsType<Conjunction>(junction);
            Assert.Equal(WhereOperator.And, junction.GetWhereOperator());
        }        

        [Fact]
        public void TestCreateDisjuctionCorrectly()
        {
            IJunction junction = Restrictions.Disjunction();
            
            Assert.IsType<Disjunction>(junction);
            Assert.Equal(WhereOperator.Or, junction.GetWhereOperator());
        }        

        private void AssertCriterion(ICriterion criterion, string propertyName, WhereOperator whereOperator, params object[] values) 
        {
            Assert.Equal(propertyName, criterion.GetPropertyName());
            Assert.Equal(whereOperator, criterion.GetWhereOperator());
            // Assert.Equal(values, criterion.GetValues());
        }
    }
}