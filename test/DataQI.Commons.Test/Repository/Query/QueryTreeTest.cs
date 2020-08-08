using System;
using System.Collections.Generic;

using DataQI.Commons.Repository.Query;
using static DataQI.Commons.Repository.Query.QueryTree;

using ExpectedObjects;
using Xunit;

namespace DataQI.Commons.Test.Repository.Query
{
    public class QueryTreeTest
    {
        [Fact]
        public void TestRejectsInvalidSource()
        {
            Assert.Throws<ArgumentException>(() => new QueryTree(null));
            Assert.Throws<ArgumentException>(() => new QueryTree(""));
        }

       [Fact]
        public void TestParsesSimplePropertyCorrectly()
        {
            var queryTree = new QueryTree("FirstName");
            AssertTree(queryTree, Members("FirstName"));
        }

        [Fact]
        public void TestParsesAndPropertiesCorrectly()
        {
            var queryTree = new QueryTree("FirstNameAndLastName");
            AssertTree(queryTree, Members("FirstName", "LastName"));
        }

        [Fact]
        public void TestParsesOrPropertiesCorrectly()
        {
            var queryTree = new QueryTree("FirstNameOrLastName");
            AssertTree(queryTree, Members("FirstName" ), Members("LastName"));
        }

        [Fact]
        public void TestParsesCombinedAndPropertiesOrAndPropertiesCorrectly()
        {
            var queryTree = new QueryTree("FirstNameAndLastNameOrAgeAndEmail");
            AssertTree(queryTree, Members("FirstName", "LastName"), Members("Age", "Email"));
        }

        [Fact]
        public void TestDetectsPrefixCorrectly()
        {
            var queryTree = new QueryTree("FindByFirstName");
            AssertTree(queryTree, Members("FirstName"));
        }

        private QueryMember[] Members(params string[] sources)
        {
            var members = new List<QueryMember>();
            foreach(var source in sources)
                members.Add(new QueryMember(source));

            return members.ToArray();
        }

       private void AssertTree(QueryTree queryTree, params QueryMember[][] exptectedMembers)
        {
            var members = queryTree.GetEnumerator();

            foreach (var expectedMember in exptectedMembers)
            {
                Assert.True(members.MoveNext());
                AssertMembers(members.Current, expectedMember);
            }

            Assert.False(members.MoveNext());
        }

        private void AssertMembers(OrQueryMember member, QueryMember[] exptectedMembers)
        {
            var members = member.GetEnumerator();

            foreach (var expectedCriterion in exptectedMembers)
            {
                Assert.True(members.MoveNext());
                expectedCriterion.ToExpectedObject().ShouldMatch(members.Current);
            }

            Assert.False(members.MoveNext());
        }
    }
}