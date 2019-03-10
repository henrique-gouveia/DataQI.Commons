using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

using Net.Data.Commons.Repository.Query;

namespace Net.Data.Commons.Repository.Core
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

            if (defaultRepository == null)
                throw new ArgumentException("Repository must not be null");

            RegisterDefaultMethods();
        }

        public static TRepository Create<TRepository>(Func<Object> defaultRepositoryFactory)
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
                FormattableString whereClause = CreateWhereClause(targetMethod.Name, args);
                dynamic parameters = createParameters(args);

                return method.Invoke(defaultRepository, new object[] { whereClause, parameters });
            }
            
            return null;
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

        protected FormattableString CreateWhereClause(string methodName, object[] args)
        {
            FormattableString whereClause = $"Name = @name";
            
            // var extractor = new CriterionExtractor(methodName);
            // var orCriterions = extractor.GetEnumerator();
            
            // while(orCriterions.MoveNext())
            // {
            //     var criterions = orCriterions.Current.GetEnumerator();
            //     while(criterions.MoveNext())
            //     {
                    
            //     }
            // }

            return whereClause;
        }

        protected dynamic createParameters(object[] args)
        {
            dynamic parameters = new ExpandoObject();
            foreach (var arg in args)
            {
                var parametersDictionary = (IDictionary<string, object>) parameters;
                parametersDictionary.Add(arg.GetType().Name, arg);
            }

            return parameters;
        }
    }
}