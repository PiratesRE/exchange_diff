using System;

namespace Microsoft.Exchange.Diagnostics.Components.HygieneDarScheduler
{
	public static class ExTraceGlobals
	{
		public static Trace HygieneDarSchedulerTracer
		{
			get
			{
				if (ExTraceGlobals.hygieneDarSchedulerTracer == null)
				{
					ExTraceGlobals.hygieneDarSchedulerTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.hygieneDarSchedulerTracer;
			}
		}

		private static Guid componentGuid = new Guid("C4515B23-59C8-422C-8B14-852E8C7B3268");

		private static Trace hygieneDarSchedulerTracer = null;
	}
}
