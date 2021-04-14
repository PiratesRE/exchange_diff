using System;

namespace Microsoft.Exchange.Diagnostics.Performance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPerformanceDataLogger
	{
		void Log(string marker, string counter, TimeSpan dataPoint);

		void Log(string marker, string counter, uint dataPoint);

		void Log(string marker, string counter, string dataPoint);
	}
}
