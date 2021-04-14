using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	internal class TimeTracer : DisposableBase
	{
		public TimeTracer(string text)
		{
			this.text = text;
			this.start = Stopwatch.GetTimestamp();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<TimeTracer>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			this.end = Stopwatch.GetTimestamp();
			if (ExTraceGlobals.RopTimingTracer.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				ExTraceGlobals.RopTimingTracer.TracePerformance<string, TimeSpan>(0L, "{0}: Operation took {1}", this.text, TimeTracer.GetTimeSpanFromTicks(this.end - this.start));
			}
		}

		private static TimeSpan GetTimeSpanFromTicks(long ticks)
		{
			double num = (double)ticks / (double)TimeTracer.frequency;
			return new TimeSpan((long)((int)(num * 10000000.0)));
		}

		private static long frequency = Stopwatch.Frequency;

		private long start;

		private long end;

		private string text;
	}
}
