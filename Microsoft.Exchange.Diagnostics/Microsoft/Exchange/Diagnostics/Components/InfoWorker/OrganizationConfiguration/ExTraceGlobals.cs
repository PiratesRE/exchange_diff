using System;

namespace Microsoft.Exchange.Diagnostics.Components.InfoWorker.OrganizationConfiguration
{
	public static class ExTraceGlobals
	{
		public static Trace OrganizationConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.organizationConfigurationTracer == null)
				{
					ExTraceGlobals.organizationConfigurationTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.organizationConfigurationTracer;
			}
		}

		private static Guid componentGuid = new Guid("2D64C97D-8957-48fb-B734-A6D176C1EA05");

		private static Trace organizationConfigurationTracer = null;
	}
}
