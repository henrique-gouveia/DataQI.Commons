using System.Collections.Generic;
using System.Linq;

namespace DataQI.Commons.Test.Repository.Sample
{
    public class CustomFakeRepository
    {
        private readonly IEnumerable<FakeEntity> fakeEntities;

        public CustomFakeRepository() : this(Enumerable.Empty<FakeEntity>())
        { }

        public CustomFakeRepository(IEnumerable<FakeEntity> fakeEntities)
            => this.fakeEntities = fakeEntities;

        public IQueryable<FakeEntity> Query()
            => fakeEntities.AsQueryable();
    }
}
