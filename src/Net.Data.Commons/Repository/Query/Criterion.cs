namespace Net.Data.Commons.Repository.Query
{
    public class Criterion
    {
        public Criterion(string source)
        {
            Type = CriterionTypeHelper.FromProperty(source);
            PropertyName = CriterionTypeHelper.ExtractProperty(source, Type);
        }

        public string PropertyName { get; private set; }

        public CriterionType Type { get; private set; }
    }
}