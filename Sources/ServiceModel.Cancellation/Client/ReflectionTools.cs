using System;
using System.Reflection;
using ServiceModel.Cancellation.Internal;

namespace ServiceModel.Cancellation.Client
{
    internal static class ReflectionTools
    {
        public static TDelegate AsStatic<TDelegate>(this MethodInfo method)
        {
            method.IsNotNull(nameof(method));

            object result = Delegate.CreateDelegate(typeof(TDelegate), method);
            return (TDelegate)result;
        }

        public static void ParseMethodName(string input, out string declaringTypeName, out string methodName)
        {
            input.IsNotNullAndNotEmpty(nameof(input));

            if (!TryParseMethodName(input, out declaringTypeName, out methodName))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(input),
                    Resources.ParseFullMethodNameInvalidInput0.FormatWith(input));
            }
        }

        private static bool TryParseMethodName(string input, out string declaringTypeName, out string methodName)
        {
            declaringTypeName = null;
            methodName = null;

            // class.method, assembly => class.method + assembly
            if (!Split(input, ',', false, out var className, out var assemblyName))
            {
                return false;
            }

            // class.method => class + method
            if (!Split(className, '.', true, out declaringTypeName, out methodName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(methodName))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(assemblyName))
            {
                declaringTypeName += ", " + assemblyName;
            }

            return true;
        }

        private static bool Split(string input, char separator, bool last, out string left, out string right)
        {
            left = null;
            right = null;

            var index = last ? input.LastIndexOf(separator) : input.IndexOf(separator);
            if (index == 0 || index == input.Length - 1)
            {
                return false;
            }

            if (index > 0)
            {
                left = input.Substring(0, index).Trim();
                right = input.Substring(index + 1).Trim();
            }
            else
            {
                left = input.Trim();
            }

            return !string.IsNullOrEmpty(left);
        }
    }
}
