using System;

namespace Microsoft.Exchange.Diagnostics.Components.TenantAttribution
{
	public static class ExTraceGlobals
	{
		public static Trace TenantAttributionInboundTracer
		{
			get
			{
				if (ExTraceGlobals.tenantAttributionInboundTracer == null)
				{
					ExTraceGlobals.tenantAttributionInboundTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.tenantAttributionInboundTracer;
			}
		}

		public static Trace TenantAttributionOutboundTracer
		{
			get
			{
				if (ExTraceGlobals.tenantAttributionOutboundTracer == null)
				{
					ExTraceGlobals.tenantAttributionOutboundTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.tenantAttributionOutboundTracer;
			}
		}

		private static Guid componentGuid = new Guid("97680724-6FF7-4C3A-BD8F-6E329E54AF3A");

		private static Trace tenantAttributionInboundTracer = null;

		private static Trace tenantAttributionOutboundTracer = null;
	}
}
