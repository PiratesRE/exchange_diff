using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal static class TimeProvider
	{
		public static DateTime UtcNow
		{
			get
			{
				if (TimeProvider.CurrentProvider != null)
				{
					return TimeProvider.CurrentProvider.UtcNow;
				}
				return DateTime.UtcNow;
			}
		}

		public static ITimeProvider SetProvider(ITimeProvider provider)
		{
			ITimeProvider currentProvider = TimeProvider.CurrentProvider;
			TimeProvider.CurrentProvider = provider;
			return currentProvider;
		}

		public static ITimeProvider CurrentProvider { get; private set; }
	}
}
