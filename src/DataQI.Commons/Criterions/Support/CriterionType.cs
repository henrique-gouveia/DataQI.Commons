using System;
using System.Text.RegularExpressions;

using DataQI.Commons.Extensions.Reflection;

namespace DataQI.Commons.Criterions.Support
{
    public enum CriterionType
    {
        [CriterionTypeData(2, "{0} BETWEEN {1} AND {2}")]
        Between, 
        [CriterionTypeData(2, "{0} NOT BETWEEN {1} AND {2}")]
        NotBetween, 

        [CriterionTypeData(1, "{0} = {1}")]
        Equals, 
        [CriterionTypeData(1, "{0} <> {1}")]
        NotEquals,

        [CriterionTypeData(1, "{0} > {1}")]
        GreaterThan,
        [CriterionTypeData(1, "{0} >= {1}")]
        GreaterThanEqual, 

        [CriterionTypeData(1, "{0} IN {1}")]
        In, 
        [CriterionTypeData(1, "{0} NOT IN {1}")]
        NotIn, 

        [CriterionTypeData(0, "{0} IS NULL")]
        IsNull, 
        [CriterionTypeData(0, "{0} IS NOT NULL")]
        IsNotNull, 

        [CriterionTypeData(1, "{0} < {1}")]
        LessThan, 
        [CriterionTypeData(1, "{0} <= {1}")]
        LessThanEqual, 

        [CriterionTypeData(1, "{0} LIKE {1}")]
        Like,
        [CriterionTypeData(1, "{0} NOT LIKE {1}")]
        NotLike,

        [CriterionTypeData(1, "{0} = {1}")]
        SimpleProperty
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class CriterionTypeDataAttribute : Attribute
    {
        public CriterionTypeDataAttribute(int numberOfArgs, string commandTemplate)
        {
            NumberOfArgs = numberOfArgs;
            CommandTemplate = commandTemplate;
        }

        public int NumberOfArgs { get; private set; }

        public string CommandTemplate { get; private set; }
    }

    public static class CriterionTypeHelper
    {
        // Full Expression: "(Is)?(Not)?(Null|Equals|Between|Containing|In|Like|((End|Start)+ingWith))|(Less|Greater)+Than(Equal)?"
        private static readonly string TYPE_TEMPLATE = "(Is)?(Not)?(Null|Equals|Between|In|Like)|(Less|Greater)+Than(Equal)?";

        private static readonly Regex TYPE_REGEX = new Regex(TYPE_TEMPLATE, RegexOptions.Compiled);

        public static CriterionType FromProperty(string source) 
        {
            var matcher = TYPE_REGEX.Match(source);
            if (matcher.Success)
            {
                if (TryFromName(matcher.Value, out var type))
                    return type;
            }

            return CriterionType.SimpleProperty;
        }

        public static bool TryFromName(string name, out CriterionType type)
        {
            return Enum.TryParse<CriterionType>(name, out type);
        }

        public static string ExtractProperty(string source, CriterionType type)
        {
            if (type == CriterionType.SimpleProperty)
                return source;
            else
                return source.Substring(0, source.IndexOf(type.ToString()));
        }

        public static CriterionTypeDataAttribute ExtractCriterionTypeData(CriterionType type) 
        {
            if (type.TryGetAttribute<CriterionTypeDataAttribute>(out var data))
                return data;

            throw new ArgumentException("Type not contains CriterionTypeData annotation");
        }
    }

    public static class CriterionTypeExtensions 
    {
        public static int NumberOfArgs(this CriterionType type)
        {
            var data = CriterionTypeHelper.ExtractCriterionTypeData(type);
            return data.NumberOfArgs;
        }

        public static string CommandTemplate(this CriterionType type)
        {
            var data = CriterionTypeHelper.ExtractCriterionTypeData(type);
            return data.CommandTemplate;
        }
    }
}