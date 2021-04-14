using System;

namespace Microsoft.Exchange.Diagnostics.Components.AutodiscoverV2
{
	public static class ExTraceGlobals
	{
		public static Trace LatencyTracer
		{
			get
			{
				if (ExTraceGlobals.latencyTracer == null)
				{
					ExTraceGlobals.latencyTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.latencyTracer;
			}
		}

		private static Guid componentGuid = new Guid("7C505D81-27E6-4F26-8B18-5BA811425E5F");

		private static Trace latencyTracer = null;
	}
}
