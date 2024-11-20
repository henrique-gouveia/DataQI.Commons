using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using DataQI.Commons.Util;

namespace DataQI.Commons.Repository.Query
{
    public class QueryMember
    {
        private static readonly string IsNotSmallLettersPattern = "(?!([a-z]))";
        private static readonly string LessOrGreaterGroupPattern = "(Less|Greater)+Than(Equal)?";
        private static readonly string LikeGroupPattern = "Contain(s|ing)|Like|((End|Start)+(s|ing)With)";
        private static readonly string NotPattern = "Not";
        private static readonly string NotGroupPattern = $"({NotPattern})?(Null|Equal|Between|In|{LikeGroupPattern})";
        private static readonly string TypePattern = $"((Is)?({NotGroupPattern}|{LessOrGreaterGroupPattern})){IsNotSmallLettersPattern}";

        private static readonly Regex NotMatcher = new Regex($"{NotPattern}{IsNotSmallLettersPattern}");
        private static readonly Regex TypeMatcher = new Regex(TypePattern);

        private static readonly IReadOnlyDictionary<MemberType, HashSet<string>> MemberTypeKeywordsMap = new Dictionary<MemberType, HashSet<string>>
        {
            { MemberType.Between, new HashSet<string>(new [] { "IsBetween", "Between" }) },
            { MemberType.Containing, new HashSet<string>(new [] { "IsContaining", "Containing", "Contains" }) },
            { MemberType.EndingWith, new HashSet<string>(new [] { "IsEndingWith", "EndingWith", "EndsWith" }) },
            { MemberType.Equal, new HashSet<string>(new [] { "IsEqual", "Equal" }) },
            { MemberType.GreaterThan, new HashSet<string>(new [] { "IsGreaterThan", "GreaterThan" }) },
            { MemberType.GreaterThanEqual, new HashSet<string>(new []{ "IsGreaterThanEqual" , "GreaterThanEqual" }) },
            { MemberType.In, new HashSet<string>(new [] { "IsIn", "In" }) },
            { MemberType.LessThan, new HashSet<string>(new []{ "IsLessThan", "LessThan" }) },
            { MemberType.LessThanEqual, new HashSet<string>(new []{ "IsLessThanEqual", "LessThanEqual" }) },
            { MemberType.Like, new HashSet<string>(new [] { "IsLike", "Like" }) },
            { MemberType.Null, new HashSet<string>(new [] { "IsNull", "Null" }) },
            { MemberType.StartingWith, new HashSet<string>(new [] { "IsStartingWith", "StartingWith", "StartsWith" }) },
        };

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
            return MemberTypeKeywordsMap
                .Where(kp => typeMatch.Success && kp.Value.Contains(typeMatch.Value))
                .Select(kp => kp.Key)
                .DefaultIfEmpty(MemberType.SimpleProperty)
                .FirstOrDefault();
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