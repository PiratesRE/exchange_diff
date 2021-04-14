using System;

namespace Microsoft.Exchange.Diagnostics.Components.FfoSelfRecoveryFx
{
	public static class ExTraceGlobals
	{
		public static Trace CommonTracer
		{
			get
			{
				if (ExTraceGlobals.commonTracer == null)
				{
					ExTraceGlobals.commonTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.commonTracer;
			}
		}

		public static Trace RAAServiceTracer
		{
			get
			{
				if (ExTraceGlobals.rAAServiceTracer == null)
				{
					ExTraceGlobals.rAAServiceTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.rAAServiceTracer;
			}
		}

		public static Trace RAANetworkTracer
		{
			get
			{
				if (ExTraceGlobals.rAANetworkTracer == null)
				{
					ExTraceGlobals.rAANetworkTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.rAANetworkTracer;
			}
		}

		private static Guid componentGuid = new Guid("2D55856F-B3DB-4318-9CB2-6B8921CEBFCB");

		private static Trace commonTracer = null;

		private static Trace rAAServiceTracer = null;

		private static Trace rAANetworkTracer = null;
	}
}
