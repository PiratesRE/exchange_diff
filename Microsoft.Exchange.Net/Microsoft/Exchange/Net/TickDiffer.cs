using System;

namespace Microsoft.Exchange.Net
{
	internal static class TickDiffer
	{
		public static TimeSpan Elapsed(int oldTicks)
		{
			return TickDiffer.Elapsed(oldTicks, Environment.TickCount);
		}

		public static TimeSpan Elapsed(int startTicks, int endTicks)
		{
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(-1 - startTicks + endTicks + 1);
			if (timeSpan >= TickDiffer.NegativeCutoff)
			{
				timeSpan = TimeSpan.FromMilliseconds(-1 - endTicks + startTicks + 1).Negate();
			}
			return timeSpan;
		}

		public static int Add(int startTicks, int amountToAdd)
		{
			return startTicks + amountToAdd;
		}

		public static int Subtract(int startTicks, int amountToSubtract)
		{
			return startTicks - amountToSubtract;
		}

		public static ulong GetTickDifference(int tickStart, int tickEnd)
		{
			long num = (long)tickStart;
			long num2 = (long)tickEnd;
			if (num2 >= num)
			{
				return (ulong)(num2 - num);
			}
			return (ulong)(2147483647L - num + (num2 - -2147483648L));
		}

		private static readonly TimeSpan NegativeCutoff = TimeSpan.FromDays(25.0);
	}
}
