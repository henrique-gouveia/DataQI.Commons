using System;
using System.Linq;
using System.Reflection;

using Net.Data.Commons.Util;

namespace Net.Data.Commons.Repository.Core
{
    public class RepositoryMetadata
    {
        private const string BASE_REPOSITORY = "ICrudRepository";

        private readonly Type repositoryInterface;

        public Type EntityType { get; private set; }

        public Type TypeId { get; private set; }

        public RepositoryMetadata(Type repositoryInterface)
        {
            Assert.True(repositoryInterface.IsInterface, "The parameter should be interface.");

            this.repositoryInterface = repositoryInterface;
            ExtractMetadata();
        }

        private void ExtractMetadata()
        {
            if (TryExtractMetadataFromCurrentInterface(out var entityType, out var typeId))
            {
                EntityType = entityType;
                TypeId = typeId;
            }
            else
            {
                ExtractMetadataFromDefaultInterface();
            }
        }

        private bool TryExtractMetadataFromCurrentInterface(out Type entityType, out Type typeId)
        {
            if (repositoryInterface.IsGenericType)
            {
                if (repositoryInterface.GenericTypeArguments.Length > 1)
                {
                    entityType = repositoryInterface.GenericTypeArguments[0];
                    typeId = repositoryInterface.GenericTypeArguments[1];
                    return true;
                }
                else
                {
                    entityType = repositoryInterface.GenericTypeArguments[0];
                    typeId = repositoryInterface.GenericTypeArguments[0];
                    return true;
                }
            }

            entityType = null;
            typeId = null;

            return false;
        }

        private void ExtractMetadataFromDefaultInterface()
        {
            var interfaces = ((TypeInfo)repositoryInterface).ImplementedInterfaces;
            if (interfaces.Count() < 1)
                throw new InvalidOperationException($"Could not resolve entity/id type of {repositoryInterface}");

            foreach (var item in interfaces)
            {
                if (item.Name.Contains(BASE_REPOSITORY)) //if (item.GenericTypeArguments.Length >= 2)
                {
                    EntityType = item.GenericTypeArguments[0];
                    TypeId = item.GenericTypeArguments[1];
                }
            }    

            Assert.NotNull(EntityType, "Could not resolve entity type.");
            Assert.NotNull(TypeId, "Could not resolve id type.");
        }
    }
}