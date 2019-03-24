namespace Net.Data.Commons.Criteria.Support
{
    public class Disjuction : Junction
    {
        public override string GetWhereOperator()
        {
            return " OR ";
        }
    }
}