using System;
using System.Collections.Generic;
using System.Reflection;

using DataQI.Commons.Query;
using DataQI.Commons.Repository.Query;
using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Core
{
    public class RepositoryProxy : DispatchProxy
    {
        protected static Func<Object> DefaultRepositoryFactory;

        protected readonly IDictionary<string, MethodInfo> defaultMethods;

        protected readonly Object defaultRepository;

        public RepositoryProxy()
        {
            defaultMethods = new Dictionary<string, MethodInfo>();
            defaultRepository = DefaultRepositoryFactory();

            Assert.NotNull(defaultRepository, "Repository must not be null");

            RegisterDefaultMethods();
        }

        protected virtual void RegisterDefaultMethods()
        {
            RegisterDefaultMethod("Delete");
            RegisterDefaultMethod("DeleteAsync");
            RegisterDefaultMethod("Exists");
            RegisterDefaultMethod("ExistsAsync");
            RegisterDefaultMethod("Find");
            RegisterDefaultMethod("FindAsync");
            RegisterDefaultMethod("FindAll");
            RegisterDefaultMethod("FindAllAsync");
            RegisterDefaultMethod("FindOne");
            RegisterDefaultMethod("FindOneAsync");
            RegisterDefaultMethod("Insert");
            RegisterDefaultMethod("InsertAsync");
            RegisterDefaultMethod("Save");
            RegisterDefaultMethod("SaveAsync");
        }

        protected virtual void RegisterDefaultMethod(string methodName)
        {
            var method = defaultRepository.GetType().GetMethod(methodName);
            if (method != null)
                defaultMethods.Add(methodName, method);
        }        

        public static TRepository Create<TRepository>(Func<Object> defaultRepositoryFactory)
            where TRepository : class
        {
            DefaultRepositoryFactory = defaultRepositoryFactory;
            return Create<TRepository, RepositoryProxy>();
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (defaultMethods.TryGetValue(targetMethod.Name, out var method))
                return method.Invoke(defaultRepository, args);

            if (defaultMethods.TryGetValue("Find", out method))
            {
                var criteriaBuilder = CreateCriteriaBuilder(targetMethod, args);
                return method.Invoke(defaultRepository, new object[] { criteriaBuilder });
            }
            
            return null;
        }

        protected virtual Func<ICriteria, ICriteria> CreateCriteriaBuilder(MethodInfo targetMethod, object[] args)
        {
            Func<ICriteria, ICriteria> criteriaBuilder = criteria => 
            {
                var factory = new QueryFactory(targetMethod, args);
                factory.BuildCriteria(criteria);

                return criteria;
            };

            return criteriaBuilder;
        }
    }
}