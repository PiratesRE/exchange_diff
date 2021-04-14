using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal static class StandaloneFuzzing
	{
		public static bool IsEnabled
		{
			get
			{
				return StandaloneFuzzing.isFuzzingEnabled ?? false;
			}
			set
			{
				if (StandaloneFuzzing.isFuzzingEnabled != null)
				{
					throw new InvalidOperationException("Can set StandaloneFuzzing.IsEnabled only once.");
				}
				StandaloneFuzzing.isFuzzingEnabled = new bool?(value);
			}
		}

		private static bool? isFuzzingEnabled;
	}
}
