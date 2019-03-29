namespace Net.Data.Commons.Criterions.Support
{
    public class Disjuction : Junction
    {
        public override string GetWhereOperator()
        {
            return " OR ";
        }
    }
}