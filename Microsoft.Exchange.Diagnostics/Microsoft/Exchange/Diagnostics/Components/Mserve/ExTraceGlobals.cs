using System;

namespace Microsoft.Exchange.Diagnostics.Components.Mserve
{
	public static class ExTraceGlobals
	{
		public static Trace ProviderTracer
		{
			get
			{
				if (ExTraceGlobals.providerTracer == null)
				{
					ExTraceGlobals.providerTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.providerTracer;
			}
		}

		public static Trace TargetConnectionTracer
		{
			get
			{
				if (ExTraceGlobals.targetConnectionTracer == null)
				{
					ExTraceGlobals.targetConnectionTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.targetConnectionTracer;
			}
		}

		public static Trace ConfigTracer
		{
			get
			{
				if (ExTraceGlobals.configTracer == null)
				{
					ExTraceGlobals.configTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.configTracer;
			}
		}

		public static Trace DeltaSyncAPITracer
		{
			get
			{
				if (ExTraceGlobals.deltaSyncAPITracer == null)
				{
					ExTraceGlobals.deltaSyncAPITracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.deltaSyncAPITracer;
			}
		}

		public static Trace MserveCacheServiceTracer
		{
			get
			{
				if (ExTraceGlobals.mserveCacheServiceTracer == null)
				{
					ExTraceGlobals.mserveCacheServiceTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.mserveCacheServiceTracer;
			}
		}

		private static Guid componentGuid = new Guid("86790e72-3e66-4b27-b3e1-66faaa21840f");

		private static Trace providerTracer = null;

		private static Trace targetConnectionTracer = null;

		private static Trace configTracer = null;

		private static Trace deltaSyncAPITracer = null;

		private static Trace mserveCacheServiceTracer = null;
	}
}
