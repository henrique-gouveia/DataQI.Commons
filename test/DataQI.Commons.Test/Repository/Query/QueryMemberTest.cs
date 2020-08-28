using System;
using DataQI.Commons.Repository.Query;
using static DataQI.Commons.Repository.Query.QueryMember;
using Xunit;

namespace DataQI.Commons.Test.Repository.Query
{
    public class QueryMemberTest
    {
        [Fact]
        public void TestRejectsNullSource()
        {
            Assert.Throws<ArgumentException>(() => new QueryMember(null));
            Assert.Throws<ArgumentException>(() => new QueryMember(""));
        }

        [Fact]
        public void TestSupportSimplePropertyTypeCorrectly()
        {
            AssertSuportType(
                source: "firstName",
                propertyNameExptected: "firstName",
                typeExpected: MemberType.SimpleProperty,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportBeteweenTypeCorrectly()
        {
            AssertSuportType(
                source: "DateOfBirthBetween",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.Between,
                numberOfArgsExpected: 2);
        }

        [Fact]
        public void TestSupportNotBeteweenTypeCorrectly()
        {
            AssertSuportType(
                source: "DateOfBirthNotBetween",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.Between,
                numberOfArgsExpected: 2,
                hasNotExpected: true);
        }

        [Fact]
        public void TestSupportContainingTypeCorrectly()
        {
            AssertSuportType(
                source: "LastNameContaining",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.Containing,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportNotContainingTypeCorrectly()
        {
            AssertSuportType(
                source: "LastNameNotContaining",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.Containing,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }

        [Fact]
        public void TestSupportEndingWithTypeCorrectly()
        {
            AssertSuportType(
                source: "LastNameEndingWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.EndingWith,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportNotEndingWithTypeCorrectly()
        {
            AssertSuportType(
                source: "LastNameNotEndingWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.EndingWith,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }

        [Fact]
        public void TestSupportEqualTypeCorrectly()
        {
            AssertSuportType(
                source: "firstNameEqual",
                propertyNameExptected: "firstName",
                typeExpected: MemberType.Equal,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportNotEqualTypeCorrectly()
        {
            AssertSuportType(
                source: "AgeNotEqual",
                propertyNameExptected: "Age",
                typeExpected: MemberType.Equal,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }

        [Fact]
        public void TestSupportGreaterThanTypeCorrectly()
        {
            AssertSuportType(
                source: "DateOfBirthGreaterThan",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.GreaterThan,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportGreaterThanEqualTypeCorrectly()
        {
            AssertSuportType(
                source: "DateOfBirthGreaterThanEqual",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.GreaterThanEqual,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportInTypeCorrectly()
        {
            AssertSuportType(
                source: "InvoiceIdIn",
                propertyNameExptected: "InvoiceId",
                typeExpected: MemberType.In,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportNotInTypeCorrectly()
        {
            AssertSuportType(
                source: "InvoiceIdNotIn",
                propertyNameExptected: "InvoiceId",
                typeExpected: MemberType.In,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }

        [Fact]
        public void TestSupportIsNullTypeCorrectly()
        {
            AssertSuportType(
                source: "PhoneNull",
                propertyNameExptected: "Phone",
                typeExpected: MemberType.Null,
                numberOfArgsExpected: 0);
        }

        [Fact]
        public void TestSupportIsNotNullTypeCorrectly()
        {
            AssertSuportType(
                source: "PhoneNotNull",
                propertyNameExptected: "Phone",
                typeExpected: MemberType.Null,
                numberOfArgsExpected: 0,
                hasNotExpected: true);
        }

        [Fact]
        public void TestSupportLessThanTypeCorrectly()
        {
            AssertSuportType(
                source: "DateOfBirthLessThan",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.LessThan,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportLessThanEqualTypeCorrectly()
        {
            AssertSuportType(
                source: "DateOfBirthLessThanEqual",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.LessThanEqual,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportLikeTypeCorrectly()
        {
            AssertSuportType(
                source: "LastNameLike",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.Like,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportNotLikeTypeCorrectly()
        {
            AssertSuportType(
                source: "LastNameNotLike",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.Like,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }

        [Fact]
        public void TestSupportStartingWithTypeCorrectly()
        {
            AssertSuportType(
                source: "LastNameStartingWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.StartingWith,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportNotStartingWithTypeCorrectly()
        {
            AssertSuportType(
                source: "LastNameNotStartingWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.StartingWith,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }

        private void AssertSuportType(
            string source,
            string propertyNameExptected,
            MemberType typeExpected,
            int numberOfArgsExpected,
            bool hasNotExpected = false)
        {
            var member = new QueryMember(source);
            Assert.Equal(propertyNameExptected, member.PropertyName);
            Assert.Equal(typeExpected, member.Type);
            Assert.Equal(numberOfArgsExpected, member.NumberOfArgs);
            Assert.Equal(hasNotExpected, member.HasNot);
        }
    }
}