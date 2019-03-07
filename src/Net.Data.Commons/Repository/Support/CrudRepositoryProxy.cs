using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Net.Data.Commons.Repository.Support
{
    public abstract class CrudRepositoryProxy<TEntity, TId> : DispatchProxy where TEntity : class, new()
    {
        protected readonly IDictionary<string, MethodInfo> defaultMethods;

        protected readonly ICrudRepository<TEntity, TId> repository;

        public CrudRepositoryProxy()
        {
            defaultMethods = new Dictionary<string, MethodInfo>();
            repository = CreateRepository();

            if (repository == null)
                throw new ArgumentException("Repository must not be null");

            RegisterDefaultMethods();
        }

        protected abstract ICrudRepository<TEntity, TId>  CreateRepository();

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            // if (!defaultMethods.Any())
            //     throw new InvalidBuildingMethodException("Any RepositoryProxy must be created by a CrudRepositoryProxyFactory");

            if (defaultMethods.TryGetValue(targetMethod.Name, out var method))
                return method.Invoke(repository, args);

            return null;
        }

        protected virtual void RegisterDefaultMethods()
        {
            RegisterDefaultMethod("Insert");
        }

        protected virtual void RegisterDefaultMethod(string methodName)
        {
            var method = repository.GetType().GetMethod(methodName);
            if (method != null)
                defaultMethods.Add(methodName, method);
        }
   }
}