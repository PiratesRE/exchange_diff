using System;

namespace Microsoft.Exchange.Diagnostics.Components.FailFast
{
	public static class ExTraceGlobals
	{
		public static Trace FailFastCacheTracer
		{
			get
			{
				if (ExTraceGlobals.failFastCacheTracer == null)
				{
					ExTraceGlobals.failFastCacheTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.failFastCacheTracer;
			}
		}

		public static Trace FailFastModuleTracer
		{
			get
			{
				if (ExTraceGlobals.failFastModuleTracer == null)
				{
					ExTraceGlobals.failFastModuleTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.failFastModuleTracer;
			}
		}

		public static Trace FailureThrottlingTracer
		{
			get
			{
				if (ExTraceGlobals.failureThrottlingTracer == null)
				{
					ExTraceGlobals.failureThrottlingTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.failureThrottlingTracer;
			}
		}

		private static Guid componentGuid = new Guid("04E8E535-4C59-49CC-B92D-4598368E5B36");

		private static Trace failFastCacheTracer = null;

		private static Trace failFastModuleTracer = null;

		private static Trace failureThrottlingTracer = null;
	}
}
