using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Net.Data.Commons.Repository.Query
{
    public enum CriterionType
    {
        Between, NotBetween, 
        Containing, NotContaining,
        Equals, NotEquals,
        In, NotIn, 
        IsNull, IsNotNull, 
        LessThan, LessThanEqual, GreaterThan, GreaterThanEqual, 
        EndingWith, NotEndingWith, StartingWith, NotStartingWith,
        SimpleProperty
    }

    public static class CriterionTypeHelper
    {
        public static CriterionType FromProperty(string source) 
        {
            foreach (var type in CriterionTypes)
                if (source.EndsWith(type.ToString()))
                    return type;

            return CriterionType.SimpleProperty;
        }

        public static string ExtractProperty(string source, CriterionType type)
        {
            if (type == CriterionType.SimpleProperty)
                return source;
            else
                return source.Substring(0, source.IndexOf(type.ToString()));
        }

        public static IEnumerable<CriterionType> CriterionTypes => 
            Enum.GetValues(typeof(CriterionType)).Cast<CriterionType>();
    }
}