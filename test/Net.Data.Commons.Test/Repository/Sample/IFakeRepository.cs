using System;
using System.Collections.Generic;
using Net.Data.Commons.Repository;

namespace Net.Data.Commons.Test.Repository.Sample
{
    public interface IFakeRepository : IDefaultRepository<FakeEntity>
    {
         IEnumerable<FakeEntity> FindByName(string name);
    }
}