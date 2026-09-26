using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataQI.Commons.Test.Repository.Sample
{
    public interface IFakeRepository : IEntityRepository<FakeEntity>
    {
        FakeEntity NotImplementedMethod();
        IQueryable<FakeEntity> Query();

        IEnumerable<FakeEntity> FindByFirstName(string name);
        IEnumerable<FakeEntity> FindByLastNameNot(string name);

        IEnumerable<FakeEntity> FindByBirthDateBetween(DateTime start, DateTime end);
        IEnumerable<FakeEntity> FindByHireDateNotBetween(DateTime start, DateTime end);

        IEnumerable<FakeEntity> FindByTitleContaining(string name);
        IEnumerable<FakeEntity> FindByTitleNotContaining(string name);
        IEnumerable<FakeEntity> FindByTitleEndingWith(string name);
        IEnumerable<FakeEntity> FindByTitleNotEndingWith(string name);
        IEnumerable<FakeEntity> FindByTitleStartingWith(string name);
        IEnumerable<FakeEntity> FindByTitleNotStartingWith(string name);
        IEnumerable<FakeEntity> FindByTitleLike(string name);
        IEnumerable<FakeEntity> FindByTitleNotLike(string name);

        IEnumerable<FakeEntity> FindByAgeGreaterThan(int age);
        IEnumerable<FakeEntity> FindByAgeGreaterThanEqual(int age);

        IEnumerable<FakeEntity> FindByAgeLessThan(int age);
        IEnumerable<FakeEntity> FindByAgeLessThanEqual(int age);

        IEnumerable<FakeEntity> FindByCityIn(string[] status);
        IEnumerable<FakeEntity> FindByCountryNotIn(string[] status);

        IEnumerable<FakeEntity> FindByEmailNull();
        IEnumerable<FakeEntity> FindByPhoneNotNull();

        IEnumerable<FakeEntity> FindByFirstNameOrLastName(string firstName, string lastName);
        IEnumerable<FakeEntity> FindByFirstNameAndLastName(string firstName, string lastName);
        IEnumerable<FakeEntity> FindByStateAndHireDateGreaterThanEqualOrCityInAndEmailEndingWith(string state, DateTime hireDate, string[] cities, string email);

        Task<IEnumerable<FakeEntity>> FindByFirstNameAsync(string name);
        Task<IEnumerable<FakeEntity>> FindByFirstNameAsync(string name, CancellationToken cancellationToken);

        FakeEntity FindByEmail(string email);
        Task<FakeEntity> FindByEmailAsync(string email);
        Task<FakeEntity> FindByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
