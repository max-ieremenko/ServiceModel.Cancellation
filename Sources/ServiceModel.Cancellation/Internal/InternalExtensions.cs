using System;
using System.Globalization;

namespace ServiceModel.Cancellation.Internal
{
    internal static class InternalExtensions
    {
        internal static string FormatWith(this string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        internal static TService Resolve<TService>(this IServiceProvider provider)
        {
            provider.IsNotNull(nameof(provider));

            object service;
            try
            {
                service = provider.GetService(typeof(TService));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(Resources.FailToResolveService0.FormatWith(typeof(TService)), ex);
            }

            if (service == null)
            {
                throw new NotSupportedException(Resources.ProviderDoesNotSupportService1.FormatWith(provider.GetType(), typeof(TService)));
            }

            return (TService)service;
        }
    }
}
