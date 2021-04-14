using System;

namespace Microsoft.Exchange.Diagnostics.Components.CommonLogging
{
	public static class ExTraceGlobals
	{
		public static Trace LogTracer
		{
			get
			{
				if (ExTraceGlobals.logTracer == null)
				{
					ExTraceGlobals.logTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.logTracer;
			}
		}

		private static Guid componentGuid = new Guid("cdcd12e6-f300-4af2-ae55-7b090a8b9f50");

		private static Trace logTracer = null;
	}
}
