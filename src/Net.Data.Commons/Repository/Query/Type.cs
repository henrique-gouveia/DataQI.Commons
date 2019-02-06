using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Net.Data.Commons.Repository.Query
{
    public enum Type
    {
        SimpleProperty, Between, Equals, In, NotIn, OrderBy, IsNull, IsNotNull, NotBetween, LessThan,
        LessThanEqual, GreaterThan, GreaterThanEqual, StartingWith, NotStartingWith,
        EndingWith, NotEndingWith, Containing, NotContaining
    }

    public class TypeInfo
    {
        private readonly int numberOfParts;
        private readonly Type type;

        public TypeInfo(Type type)
        {
            this.type = type;
        }

        public int GetNumbersOfParts()
        {
            switch (this.type)
            {
                case Type.Between:
                    return 2;
                case Type.NotBetween:
                    return 2;
                default:
                    return 1;
            }
        }
    }

    public static class TypeExtensions
    {
        public static TypeInfo Info(this Type type)
        {
            return new TypeInfo(type);
        }
    }
}