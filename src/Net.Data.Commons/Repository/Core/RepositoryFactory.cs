using System;

namespace Net.Data.Commons.Repository.Core
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
            return RepositoryProxy.Create<TRepository>(() => customImplementation);
        }

        public RepositoryMetadata GetRepositoryMetadata(Type repositoryInterface)
        {
            return new RepositoryMetadata(repositoryInterface);
        }

        protected abstract object GetCustomImplementation(Type repositoryInterface);
    }
}