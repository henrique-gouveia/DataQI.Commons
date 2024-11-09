using System;
using System.Text.RegularExpressions;

using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Query
{
    public class QueryMember
    {
        private static readonly string IsNotSmallLettersPattern = "(?!([a-z]))";
        private static readonly string LessOrGreaterGroupPattern = "(Less|Greater)+Than(Equal)?";
        private static readonly string LikeGroupPattern = "Containing|Like|((End|Start)+ingWith)";
        private static readonly string NotPattern = "Not";
        private static readonly string NotGroupPattern = $"({NotPattern})?(Null|Equal|Between|In|{LikeGroupPattern})";
        private static readonly string TypePattern = $"((Is)?({NotGroupPattern})|{LessOrGreaterGroupPattern}){IsNotSmallLettersPattern}";

        private static readonly Regex NotMatcher = new Regex($"{NotPattern}{IsNotSmallLettersPattern}");
        private static readonly Regex TypeMatcher = new Regex(TypePattern);

        private readonly Match typeMatch;

        public QueryMember(string source)
        {
            Assert.NotNullOrEmpty(source, "Source must not be null or empty");

            HasNot = NotMatcher.IsMatch(source);
            if (HasNot) source = NotMatcher.Replace(source, "");

            typeMatch = TypeMatcher.Match(source);
            Type = TypeMatched();
            PropertyName = PropertyNameFromSource(source, Type);
        }

        private string PropertyNameFromSource(string source, MemberType type)
        {
            return type == MemberType.SimpleProperty
                ? source
                : source.Substring(0, typeMatch.Index);
        }

        private MemberType TypeMatched()
        {
            return typeMatch.Success && Enum.TryParse<MemberType>(typeMatch.Value, out var type)
                ? type
                : MemberType.SimpleProperty;
        }

        public bool HasNot { get; private set; }
        public string PropertyName { get; private set; }
        public MemberType Type { get; private set; }
        public int NumberOfArgs
        {
            get
            {
                switch (Type)
                {
                    case MemberType.Between: return 2;
                    case MemberType.Null: return 0;
                    default: return 1;
                }
            }
        }

        public enum MemberType
        {
            Between,
            Containing,
            EndingWith,
            Equal,
            GreaterThan,
            GreaterThanEqual,
            In,
            LessThan,
            LessThanEqual,
            Like,
            Null,
            StartingWith,
            SimpleProperty
        }
    }
}