using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class FailoverPerformanceTrackerBase<TOpCode>
	{
		protected FailoverPerformanceTrackerBase(string tracerName)
		{
			this.m_tracerName = tracerName;
		}

		public void RecordDuration(TOpCode opCode, TimeSpan duration)
		{
			this.m_durations[opCode] = duration;
		}

		public void RunTimedOperation(TOpCode opCode, Action operation)
		{
			bool flag = false;
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			replayStopwatch.Start();
			try
			{
				operation();
				flag = true;
			}
			finally
			{
				replayStopwatch.Stop();
				if (flag)
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string, TOpCode, string>((long)this.GetHashCode(), "{0}: Operation '{1}' completed successfully in {2}.", this.m_tracerName, opCode, replayStopwatch.ToString());
				}
				else
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, TOpCode, string>((long)this.GetHashCode(), "{0}: Operation '{1}' completed with an exception in {2}.", this.m_tracerName, opCode, replayStopwatch.ToString());
				}
				this.RecordDuration(opCode, replayStopwatch.Elapsed);
			}
		}

		protected TimeSpan GetDuration(TOpCode opCode)
		{
			if (this.m_durations.ContainsKey(opCode))
			{
				return this.m_durations[opCode];
			}
			return TimeSpan.Zero;
		}

		public abstract void LogEvent();

		protected string m_tracerName;

		private Dictionary<TOpCode, TimeSpan> m_durations = new Dictionary<TOpCode, TimeSpan>(20);
	}
}
