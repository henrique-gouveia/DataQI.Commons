using System.Linq;
using System.Text;
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
                FormattableString whereClause = CreateWhereClause(targetMethod);
                dynamic parameters = createParameters(targetMethod, args);

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

        protected FormattableString CreateWhereClause(MethodInfo targetMethod)
        {
            var methodParameters = targetMethod.GetParameters();
            int argIndex = 0;

            var whereClauseBuilder = new StringBuilder();
            
            var extractor = new CriterionExtractor(targetMethod.Name);
            var orCriterions = extractor.GetEnumerator();
            
            while(orCriterions.MoveNext())
            {
                if (whereClauseBuilder.Length > 0)
                    whereClauseBuilder.Append(" OR ");

                var criterions = orCriterions.Current.GetEnumerator();
                while(criterions.MoveNext())
                {
                    var criterion = criterions.Current;

                    var parameters = new List<string>() { criterion.PropertyName };
                    for (var i = 0; i < criterion.Type.NumberOfArgs(); i++)
                        parameters.Add($"@{methodParameters[argIndex++].Name}");
                    
                    var whereClause = string.Format(
                        criterion.Type.CommandTemplate(), 
                        parameters.Cast<object>().ToArray());

                    whereClauseBuilder.Append(whereClause);
                }
            }

            return $"{whereClauseBuilder.ToString()}";
        }

        protected dynamic createParameters(MethodInfo targetMethod, object[] args)
        {
            dynamic parameters = new ExpandoObject();
            var parametersDictionary = (IDictionary<string, object>) parameters;

            var methodParameters = targetMethod.GetParameters();
            for (var i = 0; i < methodParameters.Length; i++) 
                parametersDictionary.Add(methodParameters[i].Name, args[i]);

            return parameters;
        }
    }
}