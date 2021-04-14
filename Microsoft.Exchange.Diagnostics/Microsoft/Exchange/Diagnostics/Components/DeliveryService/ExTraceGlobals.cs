using System;

namespace Microsoft.Exchange.Diagnostics.Components.DeliveryService
{
	public static class ExTraceGlobals
	{
		public static Trace ServiceTracer
		{
			get
			{
				if (ExTraceGlobals.serviceTracer == null)
				{
					ExTraceGlobals.serviceTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.serviceTracer;
			}
		}

		public static Trace StoreDriverDeliveryTracer
		{
			get
			{
				if (ExTraceGlobals.storeDriverDeliveryTracer == null)
				{
					ExTraceGlobals.storeDriverDeliveryTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.storeDriverDeliveryTracer;
			}
		}

		private static Guid componentGuid = new Guid("AFADB38E-21D5-4937-B5A1-E30ED4615958");

		private static Trace serviceTracer = null;

		private static Trace storeDriverDeliveryTracer = null;
	}
}
