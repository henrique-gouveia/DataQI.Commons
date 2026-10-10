using System;
using System.Linq;
using System.Reflection;

using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Core
{
    /// <summary>Describes the entity and identifier types of a repository interface.</summary>
    /// <remarks>
    /// The types are read from the interface's own generic arguments (first the entity, then the identifier; with a
    /// single argument the identifier type is the entity type) or, for a non generic interface, from the
    /// <c>ICrudRepository</c> interface it implements.
    /// </remarks>
    public class RepositoryMetadata
    {
        private const string RepositoryInterfaceBaseName = "ICrudRepository";

        private readonly Type repositoryInterface;

        /// <summary>Gets the entity type.</summary>
        public Type EntityType { get; private set; }

        /// <summary>Gets the identifier type.</summary>
        public Type IdType { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="RepositoryMetadata"/> class.</summary>
        /// <param name="repositoryInterface">The repository interface to inspect.</param>
        /// <exception cref="System.ArgumentException"><paramref name="repositoryInterface"/> is not an interface, or the entity or identifier type cannot be resolved.</exception>
        /// <exception cref="InvalidOperationException">The interface implements no other interface to read the types from.</exception>
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
            var interfaces = ((TypeInfo)repositoryInterface).ImplementedInterfaces;

            if (interfaces.Count() < 1)
                throw new InvalidOperationException($"Could not resolve entity/id type of {repositoryInterface}");

            foreach (var item in interfaces)
            {
                if (item.Name.Contains(RepositoryInterfaceBaseName))
                {
                    EntityType = item.GenericTypeArguments[0];
                    IdType = item.GenericTypeArguments[1];
                }
            }    

            Assert.NotNull(EntityType, "Could not resolve entity type");
            Assert.NotNull(IdType, "Could not resolve id type");
        }
    }
}