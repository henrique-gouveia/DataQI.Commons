using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DataQI.Commons.Repository.Query;

namespace DataQI.Commons.Criterions.Support
{
    public class Conjuction : Junction
    {
        public override string GetWhereOperator()
        {
            return " AND ";
        }
    }
}