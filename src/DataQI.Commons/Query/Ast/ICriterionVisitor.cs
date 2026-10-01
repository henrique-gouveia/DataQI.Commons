namespace DataQI.Commons.Query.Ast
{
    public interface ICriterionVisitor<out T>
    {
        T Visit(Comparison comparison);
        T Visit(Between between);
        T Visit(In inCriterion);
        T Visit(IsNull isNull);
        T Visit(TextMatch textMatch);
        T Visit(Not not);
        T Visit(Junction junction);
    }
}
