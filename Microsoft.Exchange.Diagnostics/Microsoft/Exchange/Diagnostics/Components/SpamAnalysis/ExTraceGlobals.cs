using System;

namespace Microsoft.Exchange.Diagnostics.Components.SpamAnalysis
{
	public static class ExTraceGlobals
	{
		public static Trace SmtpReceiveAgentTracer
		{
			get
			{
				if (ExTraceGlobals.smtpReceiveAgentTracer == null)
				{
					ExTraceGlobals.smtpReceiveAgentTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.smtpReceiveAgentTracer;
			}
		}

		public static Trace RoutingAgentTracer
		{
			get
			{
				if (ExTraceGlobals.routingAgentTracer == null)
				{
					ExTraceGlobals.routingAgentTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.routingAgentTracer;
			}
		}

		private static Guid componentGuid = new Guid("31331149-AA27-4F2D-9B69-5B46ED4ED829");

		private static Trace smtpReceiveAgentTracer = null;

		private static Trace routingAgentTracer = null;
	}
}
