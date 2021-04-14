using System;
using System.Diagnostics;

namespace Microsoft.Exchange.HttpProxy
{
	public class LatencyTracker
	{
		public LatencyTracker(LogData logData)
		{
			if (logData == null)
			{
				throw new ArgumentNullException("logData");
			}
			this.logData = logData;
		}

		protected LatencyTracker()
		{
		}

		public virtual void LogElapsedTimeInDetailedLatencyInfo(string key)
		{
			if (DiagnosticsConfiguration.DetailedLatencyTracingEnabled.Value)
			{
				this.logData.LogElapsedTimeInDetailedLatencyInfo(key);
			}
		}

		public virtual void LogElapsedTimeInMilliseconds(LogKey key)
		{
			this.logData[key] = this.logData.GetElapsedTime();
		}

		public virtual void LogLatency(LogKey key, Action operationToTrack)
		{
			Stopwatch stopwatch = new Stopwatch();
			try
			{
				stopwatch.Start();
				operationToTrack();
			}
			finally
			{
				stopwatch.Stop();
				this.logData[key] = stopwatch.ElapsedMilliseconds;
			}
		}

		public virtual T LogLatency<T>(LogKey key, Func<T> operationToTrack)
		{
			Stopwatch stopwatch = new Stopwatch();
			T result;
			try
			{
				stopwatch.Start();
				result = operationToTrack();
			}
			finally
			{
				stopwatch.Stop();
				this.logData[key] = stopwatch.ElapsedMilliseconds;
			}
			return result;
		}

		private LogData logData;
	}
}
