using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class PerfTrace : BaseTrace
	{
		internal PerfTrace(Guid componentGuid, int traceTag) : base(componentGuid, traceTag)
		{
		}

		public void TraceEvent(IPerfEventData perfEventData)
		{
			if (ETWTrace.IsEnabled && base.IsTraceEnabled(TraceType.PerformanceTrace))
			{
				int num = 0;
				ETWTrace.WriteBinary(TraceType.PerformanceTrace, this.category, this.traceTag, perfEventData.ToBytes(), out num);
			}
		}
	}
}
