using System;

namespace Microsoft.Exchange.Diagnostics.Components.GlobalLocatorCache
{
	public static class ExTraceGlobals
	{
		public static Trace ServiceTracer
		{
			get
			{
				if (ExTraceGlobals.serviceTracer == null)
				{
					ExTraceGlobals.serviceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.serviceTracer;
			}
		}

		public static Trace ClientTracer
		{
			get
			{
				if (ExTraceGlobals.clientTracer == null)
				{
					ExTraceGlobals.clientTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.clientTracer;
			}
		}

		private static Guid componentGuid = new Guid("24319d41-f580-49c1-82dc-045116f009f1");

		private static Trace serviceTracer = null;

		private static Trace clientTracer = null;
	}
}
