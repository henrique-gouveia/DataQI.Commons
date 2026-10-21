using System;
using System.Linq;

using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Core
{
    /// <summary>Describes the entity and identifier types of a repository interface.</summary>
    /// <remarks>
    /// The types are read from the interface's own generic arguments (first the entity, then the identifier; with a
    /// single argument the identifier type is the entity type) or, for a non generic interface, from the
    /// <c>IRepository&lt;TEntity, TId&gt;</c> interface it implements.
    /// </remarks>
    public class RepositoryMetadata
    {
        private readonly Type repositoryInterface;

        /// <summary>Gets the entity type.</summary>
        public Type EntityType { get; private set; }

        /// <summary>Gets the identifier type.</summary>
        public Type IdType { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="RepositoryMetadata"/> class.</summary>
        /// <param name="repositoryInterface">The repository interface to inspect.</param>
        /// <exception cref="System.ArgumentException"><paramref name="repositoryInterface"/> is not an interface, or the entity or identifier type cannot be resolved.</exception>
        /// <exception cref="InvalidOperationException">No repository marker or more than one entity/id pair is declared.</exception>
        public RepositoryMetadata(Type repositoryInterface)
        {
            Assert.True(repositoryInterface.IsInterface, "The parameter should be an interface");

            this.repositoryInterface = repositoryInterface;
            ExtractMetadata();
        }

        private void ExtractMetadata()
        {
            if (TryExtractMetadataFromCurrentInterface(out var entityType, out var idType))
            {
                EntityType = entityType;
                IdType = idType;
            }
            else
                ExtractMetadataFromDefaultInterface();
        }

        private bool TryExtractMetadataFromCurrentInterface(out Type entityType, out Type idType)
        {
            if (repositoryInterface.IsGenericType)
            {
                if (repositoryInterface.GenericTypeArguments.Length > 1)
                {
                    entityType = repositoryInterface.GenericTypeArguments[0];
                    idType = repositoryInterface.GenericTypeArguments[1];
                    return true;
                }

                entityType = repositoryInterface.GenericTypeArguments[0];
                idType = repositoryInterface.GenericTypeArguments[0];
                return true;
            }

            entityType = null;
            idType = null;
            return false;
        }

        private void ExtractMetadataFromDefaultInterface()
        {
            var candidates = repositoryInterface.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<,>))
                .Select(i => new { Entity = i.GenericTypeArguments[0], Id = i.GenericTypeArguments[1] })
                .Distinct()
                .ToList();

            Assert.NotEmpty<InvalidOperationException>(candidates,
                $"Could not resolve entity/id type of {repositoryInterface}");
            Assert.True<InvalidOperationException>(candidates.Count <= 1,
                $"{repositoryInterface} declares more than one entity/id pair");

            EntityType = candidates[0].Entity;
            IdType = candidates[0].Id;
        }
    }
}
