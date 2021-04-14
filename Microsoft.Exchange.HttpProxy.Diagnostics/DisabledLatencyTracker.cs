using System;

namespace Microsoft.Exchange.HttpProxy
{
	public class DisabledLatencyTracker : LatencyTracker
	{
		public override void LogElapsedTimeInDetailedLatencyInfo(string key)
		{
		}

		public override void LogElapsedTimeInMilliseconds(LogKey key)
		{
		}

		public override void LogLatency(LogKey key, Action operationToTrack)
		{
			operationToTrack();
		}

		public override T LogLatency<T>(LogKey key, Func<T> operationToTrack)
		{
			return operationToTrack();
		}
	}
}
