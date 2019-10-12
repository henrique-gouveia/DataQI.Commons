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
            var findByNameMethod = FakeRepositoryQueryMehod("FindByName");
            var findByNameParameters = Parameters(KeyValuePair.Create("name", (object) "fake name"));
            var findByNameArgs = new object[] { findByNameParameters.name };

            var criteria = new CriteriaFactory(findByNameMethod, findByNameArgs).Create();

            AssertCriteria(criteria, "(Name = @name)", findByNameParameters);
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