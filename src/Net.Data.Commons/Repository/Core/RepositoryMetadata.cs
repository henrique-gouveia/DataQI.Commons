using System;
using System.Linq;
using System.Reflection;

using Net.Data.Commons.Util;

namespace Net.Data.Commons.Repository.Core
{
    public class RepositoryMetadata
    {
        private readonly Type repositoryInterface;

        public Type EntityType { get; }

        public Type TypeId { get; }

        public RepositoryMetadata(Type repositoryInterface)
        {
            Assert.True(repositoryInterface.IsInterface, "The parameter should be interface.");

            this.repositoryInterface = repositoryInterface;
            this.EntityType = ExtractDomainType();
            this.TypeId = ExtractTypeId();
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

            foreach (var item in interfaces)
                if (item.GenericTypeArguments.Length >= 2)
                    return item.GenericTypeArguments[0];

            throw new ArgumentException("The entity should be informated.");
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

            foreach (var item in interfaces)
                if (item.GenericTypeArguments.Length >= 2)
                    return item.GenericTypeArguments[1];

            throw new ArgumentException("The id should be informated.");
        }
    }
}