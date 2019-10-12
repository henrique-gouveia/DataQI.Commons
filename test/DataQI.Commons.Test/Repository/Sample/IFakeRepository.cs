using System;
using System.Collections.Generic;

using DataQI.Commons.Repository;

namespace DataQI.Commons.Test.Repository.Sample
{
    public interface IFakeRepository : IDefaultRepository<FakeEntity>
    {
         IEnumerable<FakeEntity> FindByName(string name);
    }
}