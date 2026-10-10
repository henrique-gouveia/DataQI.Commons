using System;

namespace DataQI.Commons.Util
{
    /// <summary>Provides argument guards that throw <see cref="ArgumentException"/> with a caller supplied message.</summary>
    public static class Assert
    {
        /// <summary>Ensures that an object is exactly of type <typeparamref name="T"/>.</summary>
        /// <typeparam name="T">The required runtime type.</typeparam>
        /// <param name="obj">The object to check; must not be <c>null</c>.</param>
        /// <param name="message">The exception message.</param>
        /// <exception cref="ArgumentException">The runtime type of <paramref name="obj"/> is not exactly <typeparamref name="T"/> (derived types do not match).</exception>
        public static void IsType<T>(object obj, string message)
        {
            IsType(typeof(T), obj, message);
        }

        /// <summary>Ensures that an object is exactly of the given type.</summary>
        /// <param name="type">The required runtime type.</param>
        /// <param name="obj">The object to check; must not be <c>null</c>.</param>
        /// <param name="message">The exception message.</param>
        /// <exception cref="ArgumentException">The runtime type of <paramref name="obj"/> is not exactly <paramref name="type"/> (derived types do not match).</exception>
        public static void IsType(Type type, object obj, string message)
        {
            if (!(obj.GetType() == type))
                throw new ArgumentException(message);
        }

        /// <summary>Ensures that a condition holds.</summary>
        /// <param name="expression">The condition that must be <c>true</c>.</param>
        /// <param name="message">The exception message.</param>
        /// <exception cref="ArgumentException"><paramref name="expression"/> is <c>false</c>.</exception>
        public static void True(bool expression, string message)
        {
            if (!expression)
                throw new ArgumentException(message);
        }

        /// <summary>Ensures that an object is not <c>null</c>.</summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="message">The exception message.</param>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is <c>null</c>.</exception>
        public static void NotNull(object obj, string message)
        {
            if (obj == null)
                throw new ArgumentException(message);
        }

        /// <summary>Ensures that a string is neither <c>null</c> nor empty.</summary>
        /// <param name="text">The string to check.</param>
        /// <param name="message">The exception message.</param>
        /// <exception cref="ArgumentException"><paramref name="text"/> is <c>null</c> or empty.</exception>
        public static void NotNullOrEmpty(string text, string message)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException(message);
        }
    }
}