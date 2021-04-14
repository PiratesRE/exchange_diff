using System;

namespace Microsoft.Exchange.Diagnostics.Components.MExRuntime
{
	public static class ExTraceGlobals
	{
		public static Trace InitializeTracer
		{
			get
			{
				if (ExTraceGlobals.initializeTracer == null)
				{
					ExTraceGlobals.initializeTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.initializeTracer;
			}
		}

		public static Trace DispatchTracer
		{
			get
			{
				if (ExTraceGlobals.dispatchTracer == null)
				{
					ExTraceGlobals.dispatchTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.dispatchTracer;
			}
		}

		public static Trace ShutdownTracer
		{
			get
			{
				if (ExTraceGlobals.shutdownTracer == null)
				{
					ExTraceGlobals.shutdownTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.shutdownTracer;
			}
		}

		private static Guid componentGuid = new Guid("b7916055-456d-46f6-bdd2-42ac88ccb655");

		private static Trace initializeTracer = null;

		private static Trace dispatchTracer = null;

		private static Trace shutdownTracer = null;
	}
}
