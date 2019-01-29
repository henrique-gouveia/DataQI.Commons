using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Net.Data.Commons.Test
{
    public class TestRegex
    {
        // private static readonly string KEYWORD_TEMPLATE = @"({0})(?=(\p{Lu}|\P{InBASIC_LATIN}))";

        [Fact]
        public void TestSplitAndCorrectly()
        {
            var andkewordTemplate = @"And(?=\p{Lu})"; // @"(And)(?=(\p{Lu}))";
            var text = "Text1AndText2";

            var kewordRegex = new Regex(andkewordTemplate, RegexOptions.Compiled);
            var parts = kewordRegex.Split(text);

            AssertParts(parts, "Text1", "Text2");
        }
        
        [Fact]
        public void TestSplitOrCorrectly()
        {
            var orkewordTemplate = @"Or(?=\p{Lu})"; //@"(?<!^)(?=[A-Z])"
            var text = "Text1OrText2";

            var kewordRegex = new Regex(orkewordTemplate);
            var parts = kewordRegex.Split(text);

            AssertParts(parts, "Text1", "Text2");
        }

        private void AssertParts(string[] parts, params string[] exptectedParts)
        {
            for (int i = 0; i < exptectedParts.Length; i++)
                Assert.Equal(exptectedParts[i], parts[i]);
        }
    }
}