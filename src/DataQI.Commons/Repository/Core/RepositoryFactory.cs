using System;
using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Core
{
    public abstract class RepositoryFactory
    {
        public TRepository GetRepository<TRepository>(params object[] args)
            where TRepository : class
        {
            var repositoryInstance = GetRepositoryInstance(typeof(TRepository), args);
            Assert.NotNull(repositoryInstance, "Repository Instance must not be null");

            return GetRepository<TRepository>(() => repositoryInstance);
        }

        public TRepository GetRepository<TRepository>(Func<object> repositoryFactory)
            where TRepository : class
        {
            Assert.NotNull(repositoryFactory, "Repository Factory must not be null");
            return RepositoryProxy<TRepository>.Create(repositoryFactory);
        }

        public RepositoryMetadata GetRepositoryMetadata<TRepository>()
            where TRepository : class
            => GetRepositoryMetadata(typeof(TRepository));

        public RepositoryMetadata GetRepositoryMetadata(Type repositoryType)
            => new RepositoryMetadata(repositoryType);

        protected abstract object GetRepositoryInstance(Type repositoryType, params object[] args);
    }
}