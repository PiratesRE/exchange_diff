using System;

namespace Microsoft.Exchange.Diagnostics.Components.Filtering
{
	public static class ExTraceGlobals
	{
		public static Trace ADConnectorTracer
		{
			get
			{
				if (ExTraceGlobals.aDConnectorTracer == null)
				{
					ExTraceGlobals.aDConnectorTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.aDConnectorTracer;
			}
		}

		public static Trace FilteringServiceApiTracer
		{
			get
			{
				if (ExTraceGlobals.filteringServiceApiTracer == null)
				{
					ExTraceGlobals.filteringServiceApiTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.filteringServiceApiTracer;
			}
		}

		private static Guid componentGuid = new Guid("2D0C84FD-7C17-4091-8293-86745B18C1E8");

		private static Trace aDConnectorTracer = null;

		private static Trace filteringServiceApiTracer = null;
	}
}
