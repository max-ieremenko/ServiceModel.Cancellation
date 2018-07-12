using System;
using System.Collections.Generic;

namespace ServiceModel.Cancellation.Internal
{
    internal static class ParamAssert
    {
        public static void IsNotNull<T>(this T instance, string paramName)
            where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void IsNotNullAndNotEmpty<TItem>(this ICollection<TItem> instance, string paramName)
        {
            if (instance == null || instance.Count == 0)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void IsNotNullAndNotEmpty(this string instance, string paramName)
        {
            if (string.IsNullOrEmpty(instance))
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}