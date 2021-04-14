using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	internal class CpuLatencyDetectionException : LatencyDetectionException
	{
		internal CpuLatencyDetectionException(LatencyDetectionContext trigger) : base(trigger)
		{
		}
	}
}
