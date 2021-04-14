using System;

namespace Microsoft.Exchange.Diagnostics.Components.OutboundSpamAlerting
{
	public static class ExTraceGlobals
	{
		public static Trace OutboundSpamAlertingTracer
		{
			get
			{
				if (ExTraceGlobals.outboundSpamAlertingTracer == null)
				{
					ExTraceGlobals.outboundSpamAlertingTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.outboundSpamAlertingTracer;
			}
		}

		private static Guid componentGuid = new Guid("53499FCE-2E79-4ECF-86E7-9F7C2FB652EC");

		private static Trace outboundSpamAlertingTracer = null;
	}
}
