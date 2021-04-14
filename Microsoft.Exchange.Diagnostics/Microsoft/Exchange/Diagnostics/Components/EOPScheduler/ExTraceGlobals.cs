using System;

namespace Microsoft.Exchange.Diagnostics.Components.EOPScheduler
{
	public static class ExTraceGlobals
	{
		public static Trace EOPSchedulerTracer
		{
			get
			{
				if (ExTraceGlobals.eOPSchedulerTracer == null)
				{
					ExTraceGlobals.eOPSchedulerTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.eOPSchedulerTracer;
			}
		}

		private static Guid componentGuid = new Guid("c33deb09-a5c1-4b6c-9339-a89b8786ae36");

		private static Trace eOPSchedulerTracer = null;
	}
}
