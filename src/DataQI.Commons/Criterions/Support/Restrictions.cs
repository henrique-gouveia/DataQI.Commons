using System.Collections.Generic;

namespace DataQI.Commons.Criterions.Support
{
    public static class Restrictions
    {
        public static ICriterion Between(string propertyName, string parameterNameStart, string parameterNameEnd)
        {
            return CreateCriterion(propertyName, CriterionType.Between, CreateParametersNames(parameterNameStart, parameterNameEnd));
        }

        public static ICriterion NotBetween(string propertyName, string parameterNameStart, string parameterNameEnd)
        {
            return CreateCriterion(propertyName, CriterionType.NotBetween, CreateParametersNames(parameterNameStart, parameterNameEnd));
        }

        public static ICriterion Equal(string propertyName, string parameterName)
        {
            return CreateCriterion(propertyName, CriterionType.Equals, CreateParametersNames(parameterName));
        }

        public static ICriterion NotEqual(string propertyName, string parameterName)
        {
            return CreateCriterion(propertyName, CriterionType.NotEquals, CreateParametersNames(parameterName));
        }

        public static ICriterion GreaterThan(string propertyName, string parameterName)
        {
            return CreateCriterion(propertyName, CriterionType.GreaterThan, CreateParametersNames(parameterName));
        }

        public static ICriterion GreaterThanEqual(string propertyName, string parameterName)
        {
            return CreateCriterion(propertyName, CriterionType.GreaterThanEqual, CreateParametersNames(parameterName));
        }

        public static ICriterion In(string propertyName, string parameterName)
        {
            return CreateCriterion(propertyName, CriterionType.In, CreateParametersNames(parameterName));
        }

        public static ICriterion NotIn(string propertyName, string parameterName)
        {
            return CreateCriterion(propertyName, CriterionType.NotIn, CreateParametersNames(parameterName));
        }
        
        public static ICriterion IsNull(string propertyName)
        {
            return new Criterion(propertyName, CriterionType.IsNull);
        }
        
        public static ICriterion IsNotNull(string propertyName)
        {
            return new Criterion(propertyName, CriterionType.IsNotNull);
        }

        public static ICriterion LessThan(string propertyName, string parameterName)
        {
            return CreateCriterion(propertyName, CriterionType.LessThan, CreateParametersNames(parameterName));
        }

        public static ICriterion LessThanEqual(string propertyName, string parameterName)
        {
            return CreateCriterion(propertyName, CriterionType.LessThanEqual, CreateParametersNames(parameterName));
        }        

        public static ICriterion Like(string propertyName, string parameterName)
        {
            return CreateCriterion(propertyName, CriterionType.Like, CreateParametersNames(parameterName));
        }
 
        public static ICriterion NotLike(string propertyName, string parameterName)
        {
            return CreateCriterion(propertyName, CriterionType.NotLike, CreateParametersNames(parameterName));
        }

        public static ICriterion CreateCriterion(string propertyName, CriterionType type, params string[] parametersNames)
        {
            return new Criterion(propertyName, type, parametersNames);
        }

        private static string[] CreateParametersNames(params string[] parametersNames)
        {
            var parametersNamesList = new List<string>();
            foreach (var parameterName in parametersNames)
                parametersNamesList.Add(parameterName);
            
            return parametersNamesList.ToArray();
        }

        public static IJunction Conjuction()
        {
            return new Conjuction();
        }

        public static IJunction Disjuction()
        {
            return new Disjuction();
        }        
    }
}