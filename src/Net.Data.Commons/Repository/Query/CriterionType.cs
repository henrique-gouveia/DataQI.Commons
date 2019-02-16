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
}