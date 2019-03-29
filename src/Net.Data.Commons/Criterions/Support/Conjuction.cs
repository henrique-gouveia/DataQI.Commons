using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Net.Data.Commons.Repository.Query;

namespace Net.Data.Commons.Criterions.Support
{
    public class Conjuction : Junction
    {
        public override string GetWhereOperator()
        {
            return " AND ";
        }
    }
}