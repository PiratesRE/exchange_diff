using System;

namespace Microsoft.Exchange.Diagnostics.Components.SpamFeed
{
	public static class ExTraceGlobals
	{
		public static Trace RoutingTracer
		{
			get
			{
				if (ExTraceGlobals.routingTracer == null)
				{
					ExTraceGlobals.routingTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.routingTracer;
			}
		}

		public static Trace DeliveryAgentTracer
		{
			get
			{
				if (ExTraceGlobals.deliveryAgentTracer == null)
				{
					ExTraceGlobals.deliveryAgentTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.deliveryAgentTracer;
			}
		}

		public static Trace KEStoreTracer
		{
			get
			{
				if (ExTraceGlobals.kEStoreTracer == null)
				{
					ExTraceGlobals.kEStoreTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.kEStoreTracer;
			}
		}

		private static Guid componentGuid = new Guid("4A0B58AE-577F-415c-AD4F-4C577162EBDD");

		private static Trace routingTracer = null;

		private static Trace deliveryAgentTracer = null;

		private static Trace kEStoreTracer = null;
	}
}
