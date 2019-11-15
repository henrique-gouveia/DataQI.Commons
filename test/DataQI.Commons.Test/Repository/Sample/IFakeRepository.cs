using System;
using System.Collections.Generic;

using DataQI.Commons.Repository;

namespace DataQI.Commons.Test.Repository.Sample
{
    public interface IFakeRepository : IDefaultRepository<FakeEntity>
    {
         IEnumerable<FakeEntity> FindByName(string name);
         
         IEnumerable<FakeEntity> FindByFirstNameAndLastName(string firstName, string lastName);

         IEnumerable<FakeEntity> FindByFirstNameOrLastNameAndEmail(string firstName, string lastName, string email);
         
         IEnumerable<FakeEntity> FindByFirstNameStartingWithAndLastNameContainingOrDateOfBirthBetween(string firstName, string lastName, DateTime startDateOfBirth, DateTime endDateOfBirth);
    }
}