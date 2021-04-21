using DataQI.Commons.Util;
using System;

namespace DataQI.Commons.Repository.Core
{
    public abstract class RepositoryFactory
    {
        public TRepository GetRepository<TRepository>()
            where TRepository : class
        {
            var customImplementation = GetCustomImplementation(typeof(TRepository));
            return GetRepository<TRepository>(customImplementation);
        }

        public TRepository GetRepository<TRepository>(object customImplementation)
            where TRepository : class
        {
            Assert.NotNull(customImplementation, "Custom Repository Implementation must not be null");
            return RepositoryProxy<TRepository>.Create(() => customImplementation);
        }

        public RepositoryMetadata GetRepositoryMetadata<TRepository>()
            where TRepository : class
            => GetRepositoryMetadata(typeof(TRepository));

        public RepositoryMetadata GetRepositoryMetadata(Type repositoryInterface)
            => new RepositoryMetadata(repositoryInterface);

        protected abstract object GetCustomImplementation(Type repositoryInterface);
    }
}