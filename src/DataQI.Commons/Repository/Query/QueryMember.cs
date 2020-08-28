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
            Type = TypeFromSource(source);
            PropertyName = PropertyNameFromSource(source, Type);
        }

        private string PropertyNameFromSource(string source, MemberType type)
        {
           if (type == MemberType.SimpleProperty)
                return source;
            else
                return source.Substring(0, typeMatch.Index);
        }

        private MemberType TypeFromSource(string source)
        {
            var type = MemberType.SimpleProperty;

            if (typeMatch.Success)
            {
                if (Enum.TryParse<MemberType>(typeMatch.Value, out type))
                    return type;
            }

            return type;
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