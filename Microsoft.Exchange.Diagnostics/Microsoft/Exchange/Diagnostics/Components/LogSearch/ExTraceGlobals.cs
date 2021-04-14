using System;

namespace Microsoft.Exchange.Diagnostics.Components.LogSearch
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

		public static Trace CommonTracer
		{
			get
			{
				if (ExTraceGlobals.commonTracer == null)
				{
					ExTraceGlobals.commonTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.commonTracer;
			}
		}

		private static Guid componentGuid = new Guid("6510D9BF-20C7-4c2e-A346-E1B4D1112527");

		private static Trace serviceTracer = null;

		private static Trace commonTracer = null;
	}
}
