using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal sealed class Watchdog
	{
		public static void Invoke(TimeSpan timeout, Action action)
		{
			Watchdog.Invoke<bool>(timeout, delegate()
			{
				action();
				return true;
			});
		}

		public static T Invoke<T>(TimeSpan timeout, Func<T> action)
		{
			return action();
		}
	}
}
