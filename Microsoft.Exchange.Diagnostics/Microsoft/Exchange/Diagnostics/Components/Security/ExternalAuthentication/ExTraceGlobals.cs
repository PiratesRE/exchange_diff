using System;

namespace Microsoft.Exchange.Diagnostics.Components.Security.ExternalAuthentication
{
	public static class ExTraceGlobals
	{
		public static Trace ConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.configurationTracer == null)
				{
					ExTraceGlobals.configurationTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.configurationTracer;
			}
		}

		public static Trace TargetUriResolverTracer
		{
			get
			{
				if (ExTraceGlobals.targetUriResolverTracer == null)
				{
					ExTraceGlobals.targetUriResolverTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.targetUriResolverTracer;
			}
		}

		private static Guid componentGuid = new Guid("D12C2E1E-4222-40e0-A9D4-BF4A5FA2ADC1");

		private static Trace configurationTracer = null;

		private static Trace targetUriResolverTracer = null;
	}
}
