using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

using ExpectedObjects;
using Moq;
using Xunit;

using DataQI.Commons.Criterions;
using DataQI.Commons.Repository.Query;
using DataQI.Commons.Test.Repository.Sample;

using static DataQI.Commons.Repository.Query.CriterionExtractor;

namespace DataQI.Commons.Test.Repository.Query
{
    public class CriteriaFactoryTest
    {
        [Fact]
        public void TestRejectsNullQueryMethod()
        {
            var exception = Assert.Throws<ArgumentException>(() => 
                new CriteriaFactory(null, null));
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Query Method must not be null", exceptionMessage);
        }

        [Fact]
        public void TestRejectsNullQueryMethodParameters()
        {
            var findByNameMethod = FakeRepositoryQueryMehod("FindByName");
            
            var exception = Assert.Throws<ArgumentException>(() => 
                new CriteriaFactory(findByNameMethod, null));
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Query Method Parameters must not be null", exceptionMessage);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyCorrectly()
        {
            var findByMethod = FakeRepositoryQueryMehod("FindByName");
            var findByParameters = Parameters(KeyValuePair.Create("name", (object) "fake name"));
            var findByArgs = new object[] { findByParameters.name };

            var criteria = new CriteriaFactory(findByMethod, findByArgs).Create();

            AssertCriteria(criteria, "(Name = @name)", findByParameters);
        }

        [Fact]
        public void TestCreateCriteriaAndSimplePropertiesCorrectly()
        {
            var findByMethod = FakeRepositoryQueryMehod("FindByFirstNameAndLastName");
            var findByParameters = Parameters(
                KeyValuePair.Create<string, object>("firstName", "fake name"), 
                KeyValuePair.Create<string, object>("lastName", "fake last name"));
            var findByArgs = new object[] { findByParameters.firstName, findByParameters.lastName };

            var criteria = new CriteriaFactory(findByMethod, findByArgs).Create();

            AssertCriteria(criteria, "(FirstName = @firstName AND LastName = @lastName)", findByParameters);
        }

        [Fact]
        public void TestCreateCriteriaOrAndSimplePropertiesCorrectly()
        {
            var findByMethod = FakeRepositoryQueryMehod("FindByFirstNameOrLastNameAndEmail");
            var findByParameters = Parameters(
                KeyValuePair.Create<string, object>("firstName", "fake name"), 
                KeyValuePair.Create<string, object>("lastName", "fake last name"),
                KeyValuePair.Create<string, object>("email", "fake email"));
            var findByArgs = new object[] { findByParameters.firstName, findByParameters.lastName, findByParameters.email };

            var criteria = new CriteriaFactory(findByMethod, findByArgs).Create();

            AssertCriteria(criteria, "(FirstName = @firstName) OR (LastName = @lastName AND Email = @email)", findByParameters);
        }

        [Fact]
        public void TestCreateCriteriaAndOrPropertiesCorrectly()
        {
            var findByMethod = FakeRepositoryQueryMehod("FindByFirstNameLikeAndLastNameLikeOrDateOfBirthBetween");
            var findByParameters = Parameters(
                KeyValuePair.Create<string, object>("firstName", "fake name"), 
                KeyValuePair.Create<string, object>("lastName", "fake last name"),
                KeyValuePair.Create<string, object>("startDateOfBirth", "2000-01-01"),
                KeyValuePair.Create<string, object>("endDateOfBirth", "2020-01-01"));
            var findByArgs = new object[] 
                { 
                    findByParameters.firstName, 
                    findByParameters.lastName, 
                    findByParameters.startDateOfBirth,
                    findByParameters.endDateOfBirth 
                };

            var criteria = new CriteriaFactory(findByMethod, findByArgs).Create();

            AssertCriteria(criteria, "(FirstName LIKE @firstName AND LastName LIKE @lastName) OR (DateOfBirth BETWEEN @startDateOfBirth AND @endDateOfBirth)", findByParameters);
        }

        private MethodInfo FakeRepositoryQueryMehod(string name) 
        {
            var fakeRepository = new Mock<IFakeRepository>().Object;
            var method = fakeRepository.GetType().GetMethod(name);

            return method;
        }

        private dynamic Parameters(params KeyValuePair<string, object>[] parametersKeyValue)
        {
            dynamic parameters = new ExpandoObject();
            var parametersDictionary = (IDictionary<string, object>) parameters;

            foreach (var parameter in parametersKeyValue) 
                parametersDictionary.Add(parameter.Key, parameter.Value);

            return parameters;
        }

        private void AssertCriteria(ICriteria criteria, string sqlStringExpected, object parametersExpected)
        {
            Assert.NotNull(criteria);
            Assert.Equal(sqlStringExpected, criteria.ToSqlString());
            parametersExpected.ToExpectedObject().ShouldEqual(criteria.Parameters);
        }
   }
}