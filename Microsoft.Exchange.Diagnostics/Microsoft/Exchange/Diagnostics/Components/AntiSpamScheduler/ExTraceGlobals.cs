using System;

namespace Microsoft.Exchange.Diagnostics.Components.AntiSpamScheduler
{
	public static class ExTraceGlobals
	{
		public static Trace AntiSpamSchedulerTracer
		{
			get
			{
				if (ExTraceGlobals.antiSpamSchedulerTracer == null)
				{
					ExTraceGlobals.antiSpamSchedulerTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.antiSpamSchedulerTracer;
			}
		}

		private static Guid componentGuid = new Guid("D1A9D40D-C69D-41F9-BB45-4E08192A6709");

		private static Trace antiSpamSchedulerTracer = null;
	}
}
