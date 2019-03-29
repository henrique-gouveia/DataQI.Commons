using System;
using System.Dynamic;
using System.Reflection;

using ExpectedObjects;
using Moq;
using Xunit;

using Net.Data.Commons.Criterions;
using Net.Data.Commons.Repository.Query;
using Net.Data.Commons.Test.Repository.Sample;

using static Net.Data.Commons.Repository.Query.CriterionExtractor;
using System.Collections.Generic;

namespace Net.Data.Commons.Test.Repository.Query
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
        public void TestRejectsNullMethodParameters()
        {
            var findByNameMethod = GetFakeRepositoryQueryMehod("FindByName");
            
            var exception = Assert.Throws<ArgumentException>(() => 
                new CriteriaFactory(findByNameMethod, null));
            var exceptionMessage = exception.GetBaseException().Message;

            Assert.IsType<ArgumentException>(exception.GetBaseException());
            Assert.Equal("Query Method Parameters must not be null", exceptionMessage);
        }

        [Fact]
        public void TestCreateCriteriaSimplePropertyCorrectly()
        {
            var findByNameMethod = GetFakeRepositoryQueryMehod("FindByName");
            var findByNameParameters = Parameters(KeyValuePair.Create("name", "fake name"));
            var findByNameArgs = new object[] { findByNameParameters.name };

            var criteria = new CriteriaFactory(findByNameMethod, findByNameArgs).Create();

            AssertCriteria(criteria, "(Name = @name)", findByNameParameters);
        }

        private MethodInfo GetFakeRepositoryQueryMehod(string name) 
        {
            var fakeRepository = new Mock<IFakeRepository>().Object;
            var method = fakeRepository.GetType().GetMethod(name);

            return method;
        }

        private dynamic Parameters<TValue>(params KeyValuePair<string, TValue>[] parametersKeyValue)
        {
            dynamic parameters = new ExpandoObject();
            var parametersDictionary = (IDictionary<string, TValue>) parameters;

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