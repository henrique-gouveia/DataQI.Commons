using System;
using System.Collections.Generic;
using Net.Data.Commons.Repository;

namespace Net.Data.Commons.Test.Repository.Sample
{
    public interface IFakeRepository : ICrudRepository<FakeEntity, int>
    {
         IEnumerable<FakeEntity> FindByName(string name);

         IEnumerable<FakeEntity> FindByNameOrLastName(string name, string lastName);

         IEnumerable<FakeEntity> FindByDateOfBirthBetween(DateTime dateOfBirthStart, DateTime dateOfBirthEnd);

         IEnumerable<FakeEntity> FindByNameIsNull();
    }
}