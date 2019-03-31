using System.Reflection;
using System;
using System.Linq;
using Net.Data.Commons.Util;

namespace Net.Data.Commons.Repository.Core
{
    public class RepositoryMetadata
    {
        private readonly Type repositoryInterface;

        private readonly Type entityType;

        public Type EntityType { get { return entityType; } }

        private readonly Type typeId;

        public Type TypeId { get { return typeId; } }

        public RepositoryMetadata(Type repositoryInterface)
        {
            Assert.True(repositoryInterface.IsInterface, "The parameter should be interface.");

            this.repositoryInterface = repositoryInterface;
            this.entityType = ExtractDomainType();
            this.typeId = ExtractTypeId();
        }

        private Type ExtractDomainType()
        {
            if (repositoryInterface.IsGenericType)
                return repositoryInterface.GenericTypeArguments[0];
            return ExtractDomainFromDefaultInterface();
        }

        private Type ExtractDomainFromDefaultInterface()
        {
            var interfaces = ((TypeInfo)repositoryInterface).ImplementedInterfaces;
            if (interfaces.Count() < 1)
                throw new InvalidOperationException($"Could not resolve domain type of {repositoryInterface}");
            return interfaces.First().GenericTypeArguments[0];
        }

        private Type ExtractTypeId()
        {
            if (repositoryInterface.IsGenericType)
                return repositoryInterface.GenericTypeArguments[1];
            return ExtractTypeIdFromDefaultInterface();
        }

        private Type ExtractTypeIdFromDefaultInterface()
        {
            var interfaces = ((TypeInfo)repositoryInterface).ImplementedInterfaces;
            if (interfaces.Count() < 1)
                throw new InvalidOperationException($"Could not resolve id type of {repositoryInterface}");
            return interfaces.First().GenericTypeArguments[1];
        }
    }
}