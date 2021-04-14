using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	internal class DataProviderLatencyDetectionException : LatencyDetectionException
	{
		internal DataProviderLatencyDetectionException(LatencyDetectionContext trigger, IPerformanceDataProvider provider) : base(trigger, provider)
		{
		}
	}
}
