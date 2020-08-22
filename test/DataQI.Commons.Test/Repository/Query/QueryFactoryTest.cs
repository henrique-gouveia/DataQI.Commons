using System;
using System.Collections.Generic;
using System.Reflection;

using DataQI.Commons.Query;
using DataQI.Commons.Query.Support;
using DataQI.Commons.Repository.Query;
using DataQI.Commons.Test.Repository.Sample;

using ExpectedObjects;
using Moq;
using Xunit;

namespace DataQI.Commons.Test.Repository.Query
{
    public class QueryFactoryTest
    {
        [Fact]
        public void TestRejectsNullQueryMethod()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new QueryFactory(null, null));
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Query Method must not be null", exceptionMessage);
        }

        [Fact]
        public void TestRejectsNullQueryValues()
        {
            var method = QueryMethod("FindByFirstName");

            var exception = Assert.Throws<ArgumentException>(() =>
                new QueryFactory(method, null));
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Query Values must not be null", exceptionMessage);
        }

        [Fact]
        public void TestBuildRejectsNullCriteria()
        {
            var method = QueryMethod("FindByFirstName");
            var values = QueryValues("Adams");

            var exception = Assert.Throws<ArgumentException>(() =>
                new QueryFactory(method, values).BuildCriteria(null));
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Criteria must be not null", exceptionMessage);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyBetweenCorrectly()
        {
            var start = DateTime.Now.AddYears(-1);
            var end = DateTime.Now.AddYears(1);

            var method = QueryMethod("FindByBirthDateBetween");
            var values = QueryValues(start, end);

            ICriteria expected = Criteria(Restrictions.Between("BirthDate", start, end));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyNotBetweenCorrectly()
        {
            var start = DateTime.Now.AddYears(-1);
            var end = DateTime.Now.AddYears(1);

            var method = QueryMethod("FindByHireDateNotBetween");
            var values = QueryValues(start, end);

            ICriteria expected = Criteria(Restrictions.Not(Restrictions.Between("HireDate", start, end)));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyContainingCorrectly()
        {
            var name = "Adams";

            var method = QueryMethod("FindByTitleContaining");
            var values = QueryValues(name);

            ICriteria expected = Criteria(Restrictions.Containing("Title", name));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyNotContainingCorrectly()
        {
            var name = "Manager";

            var method = QueryMethod("FindByTitleNotContaining");
            var values = QueryValues(name);

            ICriteria expected = Criteria(Restrictions.Not(Restrictions.Containing("Title", name)));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyEndingWithCorrectly()
        {
            var name = "IT";

            var method = QueryMethod("FindByTitleEndingWith");
            var values = QueryValues(name);

            ICriteria expected = Criteria(Restrictions.EndingWith("Title", name));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyNotEndingWithCorrectly()
        {
            var name = "Support Agent";

            var method = QueryMethod("FindByTitleNotEndingWith");
            var values = QueryValues(name);

            ICriteria expected = Criteria(Restrictions.Not(Restrictions.EndingWith("Title", name)));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyLikeCorrectly()
        {
            var name = "General";

            var method = QueryMethod("FindByTitleLike");
            var values = QueryValues(name);

            ICriteria expected = Criteria(Restrictions.Like("Title", name));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyNotLikeCorrectly()
        {
            var name = "Sales";

            var method = QueryMethod("FindByTitleNotLike");
            var values = QueryValues(name);

            ICriteria expected = Criteria(Restrictions.Not(Restrictions.Like("Title", name)));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyEqualCorrectly()
        {
            var name = "Adams";

            var method = QueryMethod("FindByFirstName");
            var values = QueryValues(name);

            ICriteria expected = Criteria(Restrictions.Equal("FirstName", name));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyNotEqualCorrectly()
        {
            var name = "Andrew";

            var method = QueryMethod("FindByLastNameNot");
            var values = QueryValues(name);

            ICriteria expected = Criteria(Restrictions.Not(Restrictions.Equal("ByLastName", name)));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyGreaterThanCorrectly()
        {
            var age = 30;

            var method = QueryMethod("FindByAgeGreaterThan");
            var values = QueryValues(age);

            ICriteria expected = Criteria(Restrictions.GreaterThan("Age", age));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyGreaterThanEqualCorrectly()
        {
            var age = 30;

            var method = QueryMethod("FindByAgeGreaterThanEqual");
            var values = QueryValues(age);

            ICriteria expected = Criteria(Restrictions.GreaterThanEqual("Age", age));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyLessThanCorrectly()
        {
            var age = 30;

            var method = QueryMethod("FindByAgeLessThan");
            var values = QueryValues(age);

            ICriteria expected = Criteria(Restrictions.LessThan("Age", age));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyLessThanEqualCorrectly()
        {
            var age = 30;

            var method = QueryMethod("FindByAgeLessThanEqual");
            var values = QueryValues(age);

            ICriteria expected = Criteria(Restrictions.LessThanEqual("Age", age));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyInCorrectly()
        {
            var cities = new string[] { "Fortaleza", "Barcelona", "Manchester" };

            var method = QueryMethod("FindByCityIn");
            var values = QueryValues(new object[] { cities });

            ICriteria expected = Criteria(Restrictions.In("City", cities));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyNotInCorrectly()
        {
            var countries = new string[] { "Brazil", "Espain", "England" };

            var method = QueryMethod("FindByCountryNotIn");
            var values = QueryValues(new object[] { countries });

            ICriteria expected = Criteria(Restrictions.Not(Restrictions.In("Country", countries)));
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyNullCorrectly()
        {
            var method = QueryMethod("FindByEmailNull");

            ICriteria expected = Criteria(Restrictions.Null("Email"));
            ICriteria actual = new QueryFactory(method, Array.Empty<object>()).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyNotNullCorrectly()
        {
            var method = QueryMethod("FindByPhoneNotNull");

            ICriteria expected = Criteria(Restrictions.Not(Restrictions.Null("Phone")));
            ICriteria actual = new QueryFactory(method, Array.Empty<object>()).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaDisjunctionPropertiesCorrectly()
        {
            var firstName = "First Fake Name";
            var lastName = "Last Fake Name";

            var method = QueryMethod("FindByFirstNameOrLastName");
            var values = QueryValues(firstName, lastName);

            ICriteria expected = new Criteria()
                .Add(Restrictions
                    .Disjunction()
                    .Add(Restrictions
                        .Conjunction()
                        .Add(Restrictions.Equal("FirstName", firstName))
                    )
                    .Add(Restrictions
                        .Conjunction()
                        .Add(Restrictions.Equal("LastName", lastName))
                    )
                );
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaConjunctionPropertiesCorrectly()
        {
            var firstName = "First Fake Name";
            var lastName = "Last Fake Name";

            var method = QueryMethod("FindByFirstNameAndLastName");
            var values = QueryValues(firstName, lastName);

            ICriteria expected = new Criteria()
                .Add(Restrictions
                    .Disjunction()
                    .Add(Restrictions
                        .Conjunction()
                        .Add(Restrictions.Equal("FirstName", firstName))
                        .Add(Restrictions.Equal("LastName", lastName))
                    )
                );
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        [Fact]
        public void TestCreateCriteriaCombinedJunctionPropertiesCorrectly()
        {
            var state = "CE";
            var hireDate = new DateTime(2020, 1, 1);
            var cities = new string[] { "Fortaleza", "Barcelona", "Manchester" };
            var email = "email.com";

            var method = QueryMethod("FindByStateAndHireDateGreaterThanEqualOrCityInAndEmailEndingWith");
            var values = QueryValues(state, hireDate, cities, email);

            ICriteria expected = new Criteria()
                .Add(Restrictions
                    .Disjunction()
                    .Add(Restrictions
                        .Conjunction()
                        .Add(Restrictions.Equal("State", state))
                        .Add(Restrictions.GreaterThanEqual("HireDate", hireDate))
                    )
                    .Add(Restrictions
                        .Conjunction()
                        .Add(Restrictions.In("City", cities))
                        .Add(Restrictions.EndingWith("Email", email))
                    )
                );
            ICriteria actual = new QueryFactory(method, values).CreateCriteria();

            AssertCriteria(expected, actual);
        }

        private MethodInfo QueryMethod(string name)
        {
            var fakeRepository = new Mock<IFakeRepository>().Object;
            var method = fakeRepository.GetType().GetMethod(name);

            return method;
        }

        private object[] QueryValues(params object[] values)
        {
            var valuesList = new List<Object>();

            foreach (var value in values)
                valuesList.Add(value);

            return valuesList.ToArray();
        }

        private ICriteria Criteria(ICriterion criterion)
            => new Criteria()
                .Add(Restrictions
                    .Disjunction()
                    .Add(Restrictions
                        .Conjunction()
                        .Add(criterion))
                );

        private void AssertCriteria(ICriteria expected, ICriteria actual)
            => expected.ToExpectedObject().ShouldEqual(actual);
    }
}