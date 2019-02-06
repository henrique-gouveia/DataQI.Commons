using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Net.Data.Commons.Repository.Query
{
    public enum CriterionType
    {
        SimpleProperty, Between, Equals, In, NotIn, OrderBy, IsNull, IsNotNull, NotBetween, LessThan,
        LessThanEqual, GreaterThan, GreaterThanEqual, StartingWith, NotStartingWith,
        EndingWith, NotEndingWith, Containing, NotContaining
    }

    public class CriterionTypeData
    {
        private readonly int numberOfArgs;
        private readonly CriterionType type;

        public CriterionTypeData(CriterionType type)
        {
            this.type = type;
        }

        public int GetNumbersOfArgs()
        {
            switch (this.type)
            {
                case CriterionType.Between:
                    return 2;
                case CriterionType.NotBetween:
                    return 2;
                default:
                    return 1;
            }
        }
    }

    public static class TypeExtensions
    {
        public static CriterionTypeData Info(this CriterionType type)
        {
            return new CriterionTypeData(type);
        }
    }
}