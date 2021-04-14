using System;

namespace Microsoft.Exchange.Diagnostics.Components.SharedCache
{
	public static class ExTraceGlobals
	{
		public static Trace CacheTracer
		{
			get
			{
				if (ExTraceGlobals.cacheTracer == null)
				{
					ExTraceGlobals.cacheTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.cacheTracer;
			}
		}

		public static Trace ServerTracer
		{
			get
			{
				if (ExTraceGlobals.serverTracer == null)
				{
					ExTraceGlobals.serverTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.serverTracer;
			}
		}

		public static Trace ClientTracer
		{
			get
			{
				if (ExTraceGlobals.clientTracer == null)
				{
					ExTraceGlobals.clientTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.clientTracer;
			}
		}

		private static Guid componentGuid = new Guid("E71C276F-E35F-40CB-BC7E-559CE4A9B4B3");

		private static Trace cacheTracer = null;

		private static Trace serverTracer = null;

		private static Trace clientTracer = null;
	}
}
