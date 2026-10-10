using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DataQI.Commons.Query;

namespace DataQI.Commons.Repository
{
    /// <summary>Defines create, read, update and delete operations for entities of one type.</summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TId">The identifier type of <typeparamref name="TEntity"/>.</typeparam>
    /// <remarks>
    /// Whether a change is persisted immediately or only registered (for example in a unit of work) is
    /// defined by each provider; read the remarks of the implementation you use. Disposing a repository
    /// disposes the data access object it wraps.
    /// </remarks>
    public interface ICrudRepository<TEntity, in TId> : IDisposable where TEntity : class
    {
        /// <summary>Deletes the entity with the given identifier.</summary>
        /// <param name="entity">The identifier of the entity to delete.</param>
        void Delete(TId entity);

        /// <summary>Deletes the entity with the given identifier asynchronously.</summary>
        /// <param name="entity">The identifier of the entity to delete.</param>
        /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteAsync(TId entity, CancellationToken cancellationToken = default);
    
        /// <summary>Determines whether an entity with the given identifier exists.</summary>
        /// <param name="id">The identifier to look for.</param>
        /// <returns><c>true</c> if the entity exists; otherwise, <c>false</c>.</returns>
        bool Exists(TId id);

        /// <summary>Determines whether an entity with the given identifier exists asynchronously.</summary>
        /// <param name="id">The identifier to look for.</param>
        /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
        /// <returns>A task that yields <c>true</c> if the entity exists; otherwise, <c>false</c>.</returns>
        Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>Finds the entities that match the criteria built by <paramref name="criteriaBuilder"/>.</summary>
        /// <param name="criteriaBuilder">A function that receives an empty <see cref="ICriteria"/> and returns it filled.</param>
        /// <returns>The matching entities; an empty sequence when none match.</returns>
        /// <seealso cref="DataQI.Commons.Query.Support.Restrictions"/>
        IEnumerable<TEntity> Find(Func<ICriteria, ICriteria> criteriaBuilder);
        
        /// <summary>Finds the entities that match the criteria built by <paramref name="criteriaBuilder"/> asynchronously.</summary>
        /// <param name="criteriaBuilder">A function that receives an empty <see cref="ICriteria"/> and returns it filled.</param>
        /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
        /// <returns>A task that yields the matching entities; an empty sequence when none match.</returns>
        Task<IEnumerable<TEntity>> FindAsync(Func<ICriteria, ICriteria> criteriaBuilder,
            CancellationToken cancellationToken = default);

        /// <summary>Finds all entities.</summary>
        /// <returns>Every entity of type <typeparamref name="TEntity"/>.</returns>
        IEnumerable<TEntity> FindAll();

        /// <summary>Finds all entities asynchronously.</summary>
        /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
        /// <returns>A task that yields every entity of type <typeparamref name="TEntity"/>.</returns>
        Task<IEnumerable<TEntity>> FindAllAsync(CancellationToken cancellationToken = default);

        /// <summary>Finds the entity with the given identifier.</summary>
        /// <param name="id">The identifier to look for.</param>
        /// <returns>The entity, or <c>null</c> when it does not exist.</returns>
        TEntity FindOne(TId id);

        /// <summary>Finds the entity with the given identifier asynchronously.</summary>
        /// <param name="id">The identifier to look for.</param>
        /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
        /// <returns>A task that yields the entity, or <c>null</c> when it does not exist.</returns>
        Task<TEntity> FindOneAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>Finds the single entity that matches the criteria built by <paramref name="criteriaBuilder"/>.</summary>
        /// <param name="criteriaBuilder">A function that receives an empty <see cref="ICriteria"/> and returns it filled.</param>
        /// <returns>The matching entity, or <c>null</c> when none matches.</returns>
        /// <exception cref="InvalidOperationException">More than one entity matches.</exception>
        TEntity FindOne(Func<ICriteria, ICriteria> criteriaBuilder);

        /// <summary>Finds the single entity that matches the criteria built by <paramref name="criteriaBuilder"/> asynchronously.</summary>
        /// <param name="criteriaBuilder">A function that receives an empty <see cref="ICriteria"/> and returns it filled.</param>
        /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
        /// <returns>A task that yields the matching entity, or <c>null</c> when none matches.</returns>
        /// <exception cref="InvalidOperationException">More than one entity matches.</exception>
        Task<TEntity> FindOneAsync(Func<ICriteria, ICriteria> criteriaBuilder,
            CancellationToken cancellationToken = default);

        /// <summary>Adds a new entity.</summary>
        /// <param name="entity">The entity to add.</param>
        void Insert(TEntity entity);

        /// <summary>Adds a new entity asynchronously.</summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>Inserts the entity when none with the same key exists; otherwise updates the existing one.</summary>
        /// <param name="entity">The entity to save.</param>
        void Save(TEntity entity);

        /// <summary>Inserts the entity when none with the same key exists; otherwise updates the existing one asynchronously.</summary>
        /// <param name="entity">The entity to save.</param>
        /// <param name="cancellationToken">The token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SaveAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}
