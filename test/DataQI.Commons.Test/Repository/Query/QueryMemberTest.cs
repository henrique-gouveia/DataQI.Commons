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
            AssertSupportsType(
                source: "firstName",
                propertyNameExptected: "firstName",
                typeExpected: MemberType.SimpleProperty,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportBeteweenTypeCorrectly()
        {
            AssertSupportsType(
                source: "DateOfBirthBetween",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.Between,
                numberOfArgsExpected: 2);
        }
        
        [Fact]
        public void TestSupportIsBeteweenTypeCorrectly()
        {
            AssertSupportsType(
                source: "DateOfBirthIsBetween",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.Between,
                numberOfArgsExpected: 2);
        }

        [Fact]
        public void TestSupportNotBeteweenTypeCorrectly()
        {
            AssertSupportsType(
                source: "DateOfBirthNotBetween",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.Between,
                numberOfArgsExpected: 2,
                hasNotExpected: true);
        }
        
        [Fact]
        public void TestSupportIsNotBeteweenTypeCorrectly()
        {
            AssertSupportsType(
                source: "DateOfBirthIsNotBetween",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.Between,
                numberOfArgsExpected: 2,
                hasNotExpected: true);
        }
        
        [Fact]
        public void TestSupportIsContainingTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameContaining",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.Containing,
                numberOfArgsExpected: 1);
        }
        
        [Fact]
        public void TestSupportContainingTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameContaining",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.Containing,
                numberOfArgsExpected: 1);
        }
        
        [Fact]
        public void TestSupportContainsTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameContains",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.Containing,
                numberOfArgsExpected: 1);
        }
        
        [Fact]
        public void TestSupportIsNotContainingTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameIsNotContaining",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.Containing,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }
        
        [Fact]
        public void TestSupportNotContainingTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameNotContaining",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.Containing,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }
        
        [Fact]
        public void TestSupportNotContainsTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameNotContains",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.Containing,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }
        
        [Fact]
        public void TestSupportIsEndingWithTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameIsEndingWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.EndingWith,
                numberOfArgsExpected: 1);
        }
        
        [Fact]
        public void TestSupportEndingWithTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameEndingWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.EndingWith,
                numberOfArgsExpected: 1);
        }
        
        [Fact]
        public void TestSupportEndsWithTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameEndsWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.EndingWith,
                numberOfArgsExpected: 1);
        }
        
        
        [Fact]
        public void TestSupportIsNotEndingWithTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameIsNotEndingWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.EndingWith,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }        
        
        [Fact]
        public void TestSupportNotEndingWithTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameNotEndingWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.EndingWith,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }

        [Fact]
        public void TestSupportNotEndsWithTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameNotEndsWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.EndingWith,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }

        [Fact]
        public void TestSupportIsEqualTypeCorrectly()
        {
            AssertSupportsType(
                source: "firstNameIsEqual",
                propertyNameExptected: "firstName",
                typeExpected: MemberType.Equal,
                numberOfArgsExpected: 1);
        }
        
        [Fact]
        public void TestSupportEqualTypeCorrectly()
        {
            AssertSupportsType(
                source: "firstNameEqual",
                propertyNameExptected: "firstName",
                typeExpected: MemberType.Equal,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportIsNotEqualTypeCorrectly()
        {
            AssertSupportsType(
                source: "AgeIsNotEqual",
                propertyNameExptected: "Age",
                typeExpected: MemberType.Equal,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }
        
        [Fact]
        public void TestSupportNotEqualTypeCorrectly()
        {
            AssertSupportsType(
                source: "AgeNotEqual",
                propertyNameExptected: "Age",
                typeExpected: MemberType.Equal,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }

        [Fact]
        public void TestSupportIsGreaterThanTypeCorrectly()
        {
            AssertSupportsType(
                source: "DateOfBirthIsGreaterThan",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.GreaterThan,
                numberOfArgsExpected: 1);
        }
        
        [Fact]
        public void TestSupportGreaterThanTypeCorrectly()
        {
            AssertSupportsType(
                source: "DateOfBirthGreaterThan",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.GreaterThan,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportIsGreaterThanEqualTypeCorrectly()
        {
            AssertSupportsType(
                source: "DateOfBirthIsGreaterThanEqual",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.GreaterThanEqual,
                numberOfArgsExpected: 1);
        }
        
        [Fact]
        public void TestSupportGreaterThanEqualTypeCorrectly()
        {
            AssertSupportsType(
                source: "DateOfBirthGreaterThanEqual",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.GreaterThanEqual,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportIsInTypeCorrectly()
        {
            AssertSupportsType(
                source: "InvoiceIdIsIn",
                propertyNameExptected: "InvoiceId",
                typeExpected: MemberType.In,
                numberOfArgsExpected: 1);
        }
        
        [Fact]
        public void TestSupportIsNotInTypeCorrectly()
        {
            AssertSupportsType(
                source: "InvoiceIdIsNotIn",
                propertyNameExptected: "InvoiceId",
                typeExpected: MemberType.In,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }
        
        [Fact]
        public void TestSupportNotInTypeCorrectly()
        {
            AssertSupportsType(
                source: "InvoiceIdNotIn",
                propertyNameExptected: "InvoiceId",
                typeExpected: MemberType.In,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }

        [Fact]
        public void TestSupportIsNullTypeCorrectly()
        {
            AssertSupportsType(
                source: "PhoneIsNull",
                propertyNameExptected: "Phone",
                typeExpected: MemberType.Null,
                numberOfArgsExpected: 0);
        }
        
        [Fact]
        public void TestSupportNullTypeCorrectly()
        {
            AssertSupportsType(
                source: "PhoneNull",
                propertyNameExptected: "Phone",
                typeExpected: MemberType.Null,
                numberOfArgsExpected: 0);
        }

        [Fact]
        public void TestSupportIsNotNullTypeCorrectly()
        {
            AssertSupportsType(
                source: "PhoneIsNotNull",
                propertyNameExptected: "Phone",
                typeExpected: MemberType.Null,
                numberOfArgsExpected: 0,
                hasNotExpected: true);
        }

        [Fact]
        public void TestSupportNotNullTypeCorrectly()
        {
            AssertSupportsType(
                source: "PhoneNotNull",
                propertyNameExptected: "Phone",
                typeExpected: MemberType.Null,
                numberOfArgsExpected: 0,
                hasNotExpected: true);
        }

        [Fact]
        public void TestSupportIsLessThanTypeCorrectly()
        {
            AssertSupportsType(
                source: "DateOfBirthIsLessThan",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.LessThan,
                numberOfArgsExpected: 1);
        }
        
        [Fact]
        public void TestSupportLessThanTypeCorrectly()
        {
            AssertSupportsType(
                source: "DateOfBirthLessThan",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.LessThan,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportIsLessThanEqualTypeCorrectly()
        {
            AssertSupportsType(
                source: "DateOfBirthIsLessThanEqual",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.LessThanEqual,
                numberOfArgsExpected: 1);
        }
        
        [Fact]
        public void TestSupportLessThanEqualTypeCorrectly()
        {
            AssertSupportsType(
                source: "DateOfBirthLessThanEqual",
                propertyNameExptected: "DateOfBirth",
                typeExpected: MemberType.LessThanEqual,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportIsLikeTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameIsLike",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.Like,
                numberOfArgsExpected: 1);
        }
        
        [Fact]
        public void TestSupportLikeTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameLike",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.Like,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportIsNotLikeTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameNotLike",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.Like,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }
        
        [Fact]
        public void TestSupportNotLikeTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameNotLike",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.Like,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }
                
        [Fact]
        public void TestSupportIsStartingWithTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameIsStartingWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.StartingWith,
                numberOfArgsExpected: 1);
        }        
                
        [Fact]
        public void TestSupportStartingWithTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameStartingWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.StartingWith,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportStartsWithTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameStartsWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.StartingWith,
                numberOfArgsExpected: 1);
        }

        [Fact]
        public void TestSupportIsNotStartingWithTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameIsNotStartingWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.StartingWith,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }
        
        [Fact]
        public void TestSupportNotStartingWithTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameNotStartingWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.StartingWith,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }
        
        [Fact]
        public void TestSupportNotStartsWithTypeCorrectly()
        {
            AssertSupportsType(
                source: "LastNameNotStartsWith",
                propertyNameExptected: "LastName",
                typeExpected: MemberType.StartingWith,
                numberOfArgsExpected: 1,
                hasNotExpected: true);
        }

        private static void AssertSupportsType(
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