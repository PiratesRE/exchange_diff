using System;

namespace Microsoft.Exchange.Diagnostics.Components.OutboundIpReputationCheck
{
	public static class ExTraceGlobals
	{
		public static Trace OutboundIpReputationCheckTracer
		{
			get
			{
				if (ExTraceGlobals.outboundIpReputationCheckTracer == null)
				{
					ExTraceGlobals.outboundIpReputationCheckTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.outboundIpReputationCheckTracer;
			}
		}

		private static Guid componentGuid = new Guid("89ADA561-B705-4F12-8DCC-5370ACDEDC43");

		private static Trace outboundIpReputationCheckTracer = null;
	}
}
