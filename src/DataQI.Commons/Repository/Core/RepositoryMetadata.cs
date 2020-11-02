using System;
using System.Linq;
using System.Reflection;

using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Core
{
    public class RepositoryMetadata
    {
        private const string RepositoryInterfaceBaseName = "ICrudRepository";

        private readonly Type repositoryInterface;

        public Type EntityType { get; private set; }

        public Type IdType { get; private set; }

        public RepositoryMetadata(Type repositoryInterface)
        {
            Assert.True(repositoryInterface.IsInterface, "The parameter should be interface");

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
                else
                {
                    entityType = repositoryInterface.GenericTypeArguments[0];
                    idType = repositoryInterface.GenericTypeArguments[0];
                    return true;
                }
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