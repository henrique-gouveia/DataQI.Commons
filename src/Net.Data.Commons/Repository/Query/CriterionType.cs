using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Net.Data.Commons.Repository.Query
{
    public enum CriterionType
    {
        Between, NotBetween, 
        Containing, NotContaining,
        Equals, NotEquals,
        In, NotIn, 
        IsNull, IsNotNull, 
        LessThan, LessThanEqual, GreatherThan, GreatherThanEqual, 
        EndingWith, NotEndingWith, StartingWith, NotStartingWith,
        SimpleProperty
    }

    public static class CriterionTypeHelper
    {
        private static readonly string TYPE_TEMPLATE = "(Is)?(Not)?(Null|Equals|Between|Containing|In|((End|Start)+ingWith))|(Less|Greather)+Than(Equal)?";
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
    }
}