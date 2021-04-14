using System;

namespace Microsoft.Exchange.Diagnostics.Components.Data.DirectoryCache
{
	public static class ExTraceGlobals
	{
		public static Trace SessionTracer
		{
			get
			{
				if (ExTraceGlobals.sessionTracer == null)
				{
					ExTraceGlobals.sessionTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.sessionTracer;
			}
		}

		public static Trace CacheSessionTracer
		{
			get
			{
				if (ExTraceGlobals.cacheSessionTracer == null)
				{
					ExTraceGlobals.cacheSessionTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.cacheSessionTracer;
			}
		}

		public static Trace WCFServiceEndpointTracer
		{
			get
			{
				if (ExTraceGlobals.wCFServiceEndpointTracer == null)
				{
					ExTraceGlobals.wCFServiceEndpointTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.wCFServiceEndpointTracer;
			}
		}

		public static Trace WCFClientEndpointTracer
		{
			get
			{
				if (ExTraceGlobals.wCFClientEndpointTracer == null)
				{
					ExTraceGlobals.wCFClientEndpointTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.wCFClientEndpointTracer;
			}
		}

		private static Guid componentGuid = new Guid("2550C2A5-C4F4-4358-83E4-894A370B5A20");

		private static Trace sessionTracer = null;

		private static Trace cacheSessionTracer = null;

		private static Trace wCFServiceEndpointTracer = null;

		private static Trace wCFClientEndpointTracer = null;
	}
}
