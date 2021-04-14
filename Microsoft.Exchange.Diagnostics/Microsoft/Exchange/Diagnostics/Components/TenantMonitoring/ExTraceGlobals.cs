using System;

namespace Microsoft.Exchange.Diagnostics.Components.TenantMonitoring
{
	public static class ExTraceGlobals
	{
		public static Trace DataProviderTracer
		{
			get
			{
				if (ExTraceGlobals.dataProviderTracer == null)
				{
					ExTraceGlobals.dataProviderTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.dataProviderTracer;
			}
		}

		private static Guid componentGuid = new Guid("756b7f3f-cd68-4cd8-8737-180b7ad456f8");

		private static Trace dataProviderTracer = null;
	}
}
