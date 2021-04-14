using System;

namespace Microsoft.Exchange.Diagnostics.Components.NetworkOpticsProvider
{
	public static class ExTraceGlobals
	{
		public static Trace NetworkOpticsProviderTracer
		{
			get
			{
				if (ExTraceGlobals.networkOpticsProviderTracer == null)
				{
					ExTraceGlobals.networkOpticsProviderTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.networkOpticsProviderTracer;
			}
		}

		private static Guid componentGuid = new Guid("AEAC8836-FB11-4be9-BB2E-D92EA3F4A358");

		private static Trace networkOpticsProviderTracer = null;
	}
}
