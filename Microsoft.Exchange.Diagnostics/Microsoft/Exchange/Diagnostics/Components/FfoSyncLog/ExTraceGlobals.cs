using System;

namespace Microsoft.Exchange.Diagnostics.Components.FfoSyncLog
{
	public static class ExTraceGlobals
	{
		public static Trace LogGenTracer
		{
			get
			{
				if (ExTraceGlobals.logGenTracer == null)
				{
					ExTraceGlobals.logGenTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.logGenTracer;
			}
		}

		private static Guid componentGuid = new Guid("D277223A-D26E-49cc-B5AD-2446D3B89DF1");

		private static Trace logGenTracer = null;
	}
}
