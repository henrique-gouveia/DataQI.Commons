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