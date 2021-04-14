using System;

namespace Microsoft.Exchange.Diagnostics.Components.EDiscovery
{
	public static class ExTraceGlobals
	{
		public static Trace WebServiceTracer
		{
			get
			{
				if (ExTraceGlobals.webServiceTracer == null)
				{
					ExTraceGlobals.webServiceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.webServiceTracer;
			}
		}

		private static Guid componentGuid = new Guid("5BA6CF81-B765-4105-B94C-4FBA97C742C1");

		private static Trace webServiceTracer = null;
	}
}
