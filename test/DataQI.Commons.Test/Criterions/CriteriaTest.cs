using Xunit;

using DataQI.Commons.Criterions.Support;
using DataQI.Commons.Test.Repository.Sample;

namespace DataQI.Commons.Test.Criterios
{
    public class CriteriaTest
    {
        [Fact]
        public void TestJunctionBuildSqlConjunctionSimplePropertyCorrectly()
        {
            var andJunction = Restrictions.Conjuction();
            andJunction.Add(Restrictions.Equal("FirstName", "@firstName"));

            Assert.Equal("FirstName = @firstName", andJunction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlConjunctionPropertiesCorrectly()
        {
            var andJunction = Restrictions.Conjuction();
            andJunction.Add(Restrictions.NotEqual("FirstName", "@firstName"));
            andJunction.Add(Restrictions.NotEqual("LastName", "@lastName"));

            Assert.Equal("FirstName <> @firstName AND LastName <> @lastName", andJunction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlConjuctionAndConjunctionsPropertiesCorrectly()
        {
            var andFirstNameJuction = Restrictions.Conjuction().Add(Restrictions.Equal("FirstName", "@firstName"));
            var andLastNameJuction = Restrictions.Conjuction().Add(Restrictions.NotEqual("LastName", "@lastName"));
            var andJunction = Restrictions
                .Conjuction()
                .Add(andFirstNameJuction)
                .Add(andLastNameJuction);

            Assert.Equal("(FirstName = @firstName) AND (LastName <> @lastName)", andJunction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlConjuctionOrDisjunctionsPropertiesCorrectly()
        {
            var orFirstNameJuction = Restrictions.Disjuction().Add(Restrictions.NotEqual("FirstName", "@firstName"));
            var orLastNameJuction = Restrictions.Disjuction().Add(Restrictions.Equal("LastName", "@lastName"));
            var andJunction = Restrictions
                .Conjuction()
                .Add(orFirstNameJuction)
                .Add(orLastNameJuction);

            Assert.Equal("(FirstName <> @firstName) AND (LastName = @lastName)", andJunction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlDisjunctionSimplePropertyCorrectly()
        {
            var orJunction = Restrictions.Disjuction();
            orJunction.Add(Restrictions.NotEqual("FirstName", "@firstName"));

            Assert.Equal("FirstName <> @firstName", orJunction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlDisjuctionPropertiesCorrectly()
        {
            var orJunction = Restrictions.Disjuction();
            orJunction.Add(Restrictions.Equal("FirstName", "@firstName"));
            orJunction.Add(Restrictions.Equal("LastName", "@lastName"));

            Assert.Equal("FirstName = @firstName OR LastName = @lastName", orJunction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlDisjunctionOrDisjunctionsPropertiesCorrectly()
        {
            var orFirstNameJuction = Restrictions.Disjuction().Add(Restrictions.NotEqual("FirstName", "@firstName"));
            var orLastNameJuction = Restrictions.Disjuction().Add(Restrictions.Equal("LastName", "@lastName"));
            var orJunction = Restrictions
                .Disjuction()
                .Add(orFirstNameJuction)
                .Add(orLastNameJuction);

            Assert.Equal("(FirstName <> @firstName) OR (LastName = @lastName)", orJunction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlDisjunctionAndConjunctionsPropertiesCorrectly()
        {
            var andFirstNameJuction = Restrictions.Conjuction().Add(Restrictions.Equal("FirstName", "@firstName"));
            var andLastNameJuction = Restrictions.Conjuction().Add(Restrictions.NotEqual("LastName", "@lastName"));
            var orJunction = Restrictions
                .Disjuction()
                .Add(andFirstNameJuction)
                .Add(andLastNameJuction);

            Assert.Equal("(FirstName = @firstName) OR (LastName <> @lastName)", orJunction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlBetweenCorrectly()
        {
            var junction = Restrictions.Conjuction();
            junction.Add(Restrictions.Between("DateOfBirth", "@dateOfBirthStart", "@dateOfBirthEnd"));

            Assert.Equal("DateOfBirth BETWEEN @dateOfBirthStart AND @dateOfBirthEnd", junction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlNotBetweenCorrectly()
        {
            var junction = Restrictions.Disjuction();
            junction.Add(Restrictions.NotBetween("DateOfBirth", "@dateOfBirthStart", "@dateOfBirthEnd"));

            Assert.Equal("DateOfBirth NOT BETWEEN @dateOfBirthStart AND @dateOfBirthEnd", junction.ToSqlString());
        }

        // [Fact]
        // public void TestJunctionBuildSqlContainingCorrectly()
        // {
        //     var junction = Restrictions.Disjuction();
        //     junction.Add(Restrictions.Containing("FullName", "@fullName"));

        //     Assert.Equal("FullName LIKE @fullName", junction.ToSqlString());
        // }

        // [Fact]
        // public void TestJunctionBuildSqlNotContainingCorrectly()
        // {
        //     var junction = Restrictions.Conjuction();
        //     junction.Add(Restrictions.NotContaining("FullName", "@fullName"));

        //     Assert.Equal("FullName NOT LIKE @fullName", junction.ToSqlString());
        // }

        // [Fact]
        // public void TestJunctionBuildSqlEndingWithCorrectly()
        // {
        //     var junction = Restrictions.Conjuction();
        //     junction.Add(Restrictions.EndingWith("FullName", "@fullName"));

        //     Assert.Equal("FullName LIKE @fullName", junction.ToSqlString());
        // }

        // [Fact]
        // public void TestJunctionBuildSqlNotEndingWithCorrectly()
        // {
        //     var junction = Restrictions.Disjuction();
        //     junction.Add(Restrictions.NotEndingWith("FullName", "@fullName"));

        //     Assert.Equal("FullName NOT LIKE @fullName", junction.ToSqlString());
        // }

        [Fact]
        public void TestJunctionBuildSqlGreatherThanCorrectly()
        {
            var junction = Restrictions.Disjuction();
            junction.Add(Restrictions.GreaterThan("Age", "@age"));

            Assert.Equal("Age > @age", junction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlGreatherThanEqualCorrectly()
        {
            var junction = Restrictions.Conjuction();
            junction.Add(Restrictions.GreaterThanEqual("Age", "@age"));

            Assert.Equal("Age >= @age", junction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlInCorrectly()
        {
            var junction = Restrictions.Conjuction();
            junction.Add(Restrictions.In("Age", "@age"));

            Assert.Equal("Age IN @age", junction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlNotInCorrectly()
        {
            var junction = Restrictions.Disjuction();
            junction.Add(Restrictions.NotIn("Age", "@age"));

            Assert.Equal("Age NOT IN @age", junction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlIsNullCorrectly()
        {
            var junction = Restrictions.Disjuction();
            junction.Add(Restrictions.IsNull("Mail"));

            Assert.Equal("Mail IS NULL", junction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlIsNotNullCorrectly()
        {
            var junction = Restrictions.Conjuction();
            junction.Add(Restrictions.IsNotNull("Mail"));

            Assert.Equal("Mail IS NOT NULL", junction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlLessThanCorrectly()
        {
            var junction = Restrictions.Conjuction();
            junction.Add(Restrictions.LessThan("DateOfBirth", "@dateOfBirth"));

            Assert.Equal("DateOfBirth < @dateOfBirth", junction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlLessThanEqualCorrectly()
        {
            var junction = Restrictions.Disjuction();
            junction.Add(Restrictions.LessThanEqual("DateOfBirth", "@dateOfBirth"));

            Assert.Equal("DateOfBirth <= @dateOfBirth", junction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlLikeCorrectly()
        {
            var junction = Restrictions.Disjuction();
            junction.Add(Restrictions.Like("FullName", "@fullName"));

            Assert.Equal("FullName LIKE @fullName", junction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlNotLikeCorrectly()
        {
            var junction = Restrictions.Conjuction();
            junction.Add(Restrictions.NotLike("FullName", "@fullName"));

            Assert.Equal("FullName NOT LIKE @fullName", junction.ToSqlString());
        }        

        // [Fact]
        // public void TestJunctionBuildSqlStartingWithCorrectly()
        // {
        //     var junction = Restrictions.Disjuction();
        //     junction.Add(Restrictions.StartingWith("FullName", "@fullName"));

        //     Assert.Equal("FullName LIKE @fullName", junction.ToSqlString());
        // }

        // [Fact]
        // public void TestJunctionBuildSqlNotStartingWithCorrectly()
        // {
        //     var junction = Restrictions.Disjuction();
        //     junction.Add(Restrictions.NotStartingWith("FullName", "@fullName"));

        //     Assert.Equal("FullName NOT LIKE @fullName", junction.ToSqlString());
        // }        

        [Fact]
        public void TestJunctionBuildSqlOrAndJunctionsPropertiesCorrectly()
        {
            var andJunction1 = Restrictions.Conjuction();
            andJunction1.Add(Restrictions.Equal("FirstName", "@firstName_1"));
            andJunction1.Add(Restrictions.Equal("LastName", "@lastName_1"));

            var andJunction2 = Restrictions.Conjuction();
            andJunction2.Add(Restrictions.Equal("FirstName", "@firstName_2"));
            andJunction2.Add(Restrictions.Equal("LastName", "@lastName_2"));

            var orJunction = Restrictions.Disjuction();
            orJunction.Add(andJunction1);
            orJunction.Add(andJunction2);
            
            var sqlWhereExpected = 
                "(FirstName = @firstName_1 AND LastName = @lastName_1)" 
              + " OR "
              + "(FirstName = @firstName_2 AND LastName = @lastName_2)";

            Assert.Equal(sqlWhereExpected, orJunction.ToSqlString());
        }

        [Fact]
        public void TestJunctionBuildSqlAndOrJunctionsPropertiesCorrectly()
        {
            var orJunction1 = Restrictions.Disjuction();
            orJunction1.Add(Restrictions.Equal("FirstName", "@firstName_1"));
            orJunction1.Add(Restrictions.Equal("LastName", "@lastName_1"));

            var orJunction2 = Restrictions.Disjuction();
            orJunction2.Add(Restrictions.Equal("FirstName", "@firstName_2"));
            orJunction2.Add(Restrictions.Equal("LastName", "@lastName_2"));

            var andJunction = Restrictions.Conjuction();
            andJunction.Add(orJunction1);
            andJunction.Add(orJunction2);
            
            var sqlWhereExpected = 
                "(FirstName = @firstName_1 OR LastName = @lastName_1)" 
              + " AND "
              + "(FirstName = @firstName_2 OR LastName = @lastName_2)";

            Assert.Equal(sqlWhereExpected, andJunction.ToSqlString());
        }

        [Fact]
        public void TestCriteriaBuildSqlSimplePropertyCorrectly()
        {
            var criteria = new Criteria()
                .Add(Restrictions.Equal("FirstName", "@firstName"));

            Assert.Equal("FirstName = @firstName", criteria.ToSqlString());
        }

        [Fact]
        public void TestCriteriaBuildSqlAddPropertiesCorrectly()
        {
            var criteria = new Criteria()
                .Add(Restrictions.Equal("FirstName", "@firstName"))
                .Add(Restrictions.Equal("LastName", "@lastName"));

            Assert.Equal("FirstName = @firstName AND LastName = @lastName", criteria.ToSqlString());
        }

        [Fact]
        public void TestCriteriaBuildSqlAddSimplePropertyAndSimpleConjunctionCorrectly()
        {
            var criteria = new Criteria()
                .Add(Restrictions.Equal("FirstName", "@firstName"))
                .Add(Restrictions
                    .Conjuction()
                    .Add(Restrictions.Between("DateOfBirth", "@dateOfBirthStart", "@dateOfBirthEnd"))
                    .Add(Restrictions.IsNotNull("Phone")));

            var sqlWhereExpected = 
                "FirstName = @firstName" 
              + " AND "
              + "DateOfBirth BETWEEN @dateOfBirthStart AND @dateOfBirthEnd AND Phone IS NOT NULL";

            Assert.Equal(sqlWhereExpected, criteria.ToSqlString());
        }

        [Fact]
        public void TestCriteriaBuildSqlAddPropertiesAndConjunctionCorrectly()
        {
            var criteria = new Criteria()
                .Add(Restrictions.Equal("FirstName", "@firstName"))
                .Add(Restrictions.Equal("LastName", "@lastName"))
                .Add(Restrictions
                    .Conjuction()
                    .Add(Restrictions.Between("DateOfBirth", "@dateOfBirthStart", "@dateOfBirthEnd"))
                    .Add(Restrictions.IsNotNull("Phone")));

            var sqlWhereExpected = 
                "FirstName = @firstName" 
              + " AND "
              + "LastName = @lastName"
              + " AND "
              + "DateOfBirth BETWEEN @dateOfBirthStart AND @dateOfBirthEnd AND Phone IS NOT NULL";

            Assert.Equal(sqlWhereExpected, criteria.ToSqlString());
        }

        [Fact]
        public void TestCriteriaBuildSqlAddSimplePropertyAndSimpleDisjunctionCorrectly()
        {
            var criteria = new Criteria()
                .Add(Restrictions.Equal("FirstName", "@firstName"))
                .Add(Restrictions
                    .Disjuction()
                    .Add(Restrictions.Between("DateOfBirth", "@dateOfBirthStart", "@dateOfBirthEnd"))
                    .Add(Restrictions.IsNotNull("Phone")));

            var sqlWhereExpected = 
                "FirstName = @firstName" 
              + " AND "
              + "DateOfBirth BETWEEN @dateOfBirthStart AND @dateOfBirthEnd OR Phone IS NOT NULL";

            Assert.Equal(sqlWhereExpected, criteria.ToSqlString());
        }

        [Fact]
        public void TestCriteriaBuildSqlAddPropertiesAndDisjunctionCorrectly()
        {
            var criteria = new Criteria()
                .Add(Restrictions.Equal("FirstName", "@firstName"))
                .Add(Restrictions.Equal("LastName", "@lastName"))
                .Add(Restrictions
                    .Disjuction()
                    .Add(Restrictions.Between("DateOfBirth", "@dateOfBirthStart", "@dateOfBirthEnd"))
                    .Add(Restrictions.IsNotNull("Phone")));

            var sqlWhereExpected = 
                "FirstName = @firstName" 
              + " AND "
              + "LastName = @lastName"
              + " AND "
              + "DateOfBirth BETWEEN @dateOfBirthStart AND @dateOfBirthEnd OR Phone IS NOT NULL";

            Assert.Equal(sqlWhereExpected, criteria.ToSqlString());
        }
    }
}