using System;

namespace Microsoft.Exchange.Diagnostics.Performance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NullPerformanceDataLogger : IPerformanceDataLogger
	{
		private NullPerformanceDataLogger()
		{
		}

		public void Log(string marker, string counter, TimeSpan value)
		{
		}

		public void Log(string marker, string counter, uint value)
		{
		}

		public void Log(string marker, string counter, string value)
		{
		}

		public static readonly IPerformanceDataLogger Instance = new NullPerformanceDataLogger();
	}
}
