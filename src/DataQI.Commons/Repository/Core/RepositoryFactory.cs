using System;
using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Core
{
    /// <summary>Creates repository proxies that serve both the CRUD members and name based query methods.</summary>
    public abstract class RepositoryFactory
    {
        /// <summary>Creates a repository for the interface <typeparamref name="TRepository"/>.</summary>
        /// <typeparam name="TRepository">The repository interface.</typeparam>
        /// <param name="args">The constructor arguments of the provider's repository implementation.</param>
        /// <returns>A proxy implementing <typeparamref name="TRepository"/>.</returns>
        /// <exception cref="System.ArgumentException">The provider created no repository instance.</exception>
        public TRepository GetRepository<TRepository>(params object[] args)
            where TRepository : class
        {
            var repositoryInstance = GetRepositoryInstance(typeof(TRepository), args);
            Assert.NotNull(repositoryInstance, "Repository instance must not be null");

            return GetRepository<TRepository>(() => repositoryInstance);
        }

        /// <summary>Creates a repository for the interface <typeparamref name="TRepository"/> around a custom implementation.</summary>
        /// <typeparam name="TRepository">The repository interface.</typeparam>
        /// <param name="repositoryFactory">A function that returns the implementation the proxy forwards to; must not be <c>null</c>.</param>
        /// <returns>A proxy implementing <typeparamref name="TRepository"/>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="repositoryFactory"/> is <c>null</c>.</exception>
        public TRepository GetRepository<TRepository>(Func<object> repositoryFactory)
            where TRepository : class
        {
            Assert.NotNull(repositoryFactory, "Repository factory must not be null");
            return RepositoryProxy<TRepository>.Create(repositoryFactory);
        }

        /// <summary>Gets the metadata of the repository interface <typeparamref name="TRepository"/>.</summary>
        /// <typeparam name="TRepository">The repository interface.</typeparam>
        /// <returns>The entity and identifier types of the interface.</returns>
        public RepositoryMetadata GetRepositoryMetadata<TRepository>()
            where TRepository : class
            => GetRepositoryMetadata(typeof(TRepository));

        /// <summary>Gets the metadata of a repository interface.</summary>
        /// <param name="repositoryType">The repository interface.</param>
        /// <returns>The entity and identifier types of the interface.</returns>
        public RepositoryMetadata GetRepositoryMetadata(Type repositoryType)
            => new RepositoryMetadata(repositoryType);

        /// <summary>Creates the provider's repository implementation for a repository interface.</summary>
        /// <param name="repositoryType">The repository interface.</param>
        /// <param name="args">The constructor arguments of the implementation.</param>
        /// <returns>The implementation instance the proxy forwards to.</returns>
        protected abstract object GetRepositoryInstance(Type repositoryType, params object[] args);
    }
}