using System.Reflection;
using System;
using System.Linq;
using Net.Data.Commons.Util;

namespace Net.Data.Commons.Repository.Core
{
    public class RepositoryMetadata : IRepositoryMetadata
    {
        private readonly Type repositoryInterface;

        public RepositoryMetadata(Type repositoryInterface)
        {
            Assert.IsTrue(repositoryInterface.IsInterface, "The parameter should be interface.");
            this.repositoryInterface = repositoryInterface;
        }

        public Type GetDomainType()
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

        public Type GetTypeId()
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

        public Type GetRepositoryInterface()
        {
            return repositoryInterface;
        }
    }
}