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

        IEnumerable<FakeEntity> FindBy();
        IEnumerable<FakeEntity> FindByFirstName(string name);
        IEnumerable<FakeEntity> FindByNameStartingWithAndStockGreaterThanOrDepartmentIn(string name, decimal stock, string[] department);
        IEnumerable<FakeEntity> FindByNameNotLike(string name);
        IEnumerable<FakeEntity> FindByNameIsNotNull();
        IEnumerable<FakeEntity> FindByOrderDate(DateTime date);
        IEnumerable<FakeEntity> FindByAndroidVersion(string version);
        IEnumerable<FakeEntity> FindByCategoryInStock(string[] category);
        IEnumerable<FakeEntity> FindByNameEquals(string name);

        Task<IEnumerable<FakeEntity>> FindByFirstNameAsync(string name);
        Task<IEnumerable<FakeEntity>> FindByFirstNameAsync(string name, CancellationToken cancellationToken);

        FakeEntity FindByEmail(string email);
        TResult FindByEmail<TResult>(string email);
        Task<FakeEntity> FindByEmailAsync(string email);
        Task<FakeEntity> FindByEmailAsync(string email, CancellationToken cancellationToken);
        Task<TResult> FindByEmailAsync<TResult>(string email, CancellationToken cancellationToken = default);
    }
}
