using System;

namespace Net.Data.Commons.Util
{
    public static class Assert
    {
        public static void NotNull(object obj, string message)
        {
            if (obj == null)
                throw new ArgumentNullException(message);
        }

        public static void IsTrue(bool expression, string message)
        {
            if (!expression)
                throw new ArgumentException(message);
        }

        public static void IsNullOrEmpty(string text, string message)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException(message);
        }
    }
}