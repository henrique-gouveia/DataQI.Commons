using System;

namespace Net.Data.Commons.Util
{
    public static class Assert
    {

        public static void IsNotNullOrEmpty(string text, string message)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException(message);
        }

        public static void IsType<T>(object obj, string message)
        {
            IsType(typeof(T), obj, message);
        }

        public static void IsType(Type type, object obj, string message)
        {
            if (!(obj.GetType() == type))
                throw new ArgumentException(message);
        }

        public static void IsTrue(bool expression, string message)
        {
            if (!expression)
                throw new ArgumentException(message);
        }

        public static void NotNull(object obj, string message)
        {
            if (obj == null)
                throw new ArgumentException(message);
        }
    }
}