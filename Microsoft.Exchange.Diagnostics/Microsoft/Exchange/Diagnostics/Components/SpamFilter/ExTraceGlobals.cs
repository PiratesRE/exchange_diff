using System;

namespace Microsoft.Exchange.Diagnostics.Components.SpamFilter
{
	public static class ExTraceGlobals
	{
		public static Trace AgentTracer
		{
			get
			{
				if (ExTraceGlobals.agentTracer == null)
				{
					ExTraceGlobals.agentTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.agentTracer;
			}
		}

		public static Trace BlockSendersTracer
		{
			get
			{
				if (ExTraceGlobals.blockSendersTracer == null)
				{
					ExTraceGlobals.blockSendersTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.blockSendersTracer;
			}
		}

		public static Trace SafeSendersTracer
		{
			get
			{
				if (ExTraceGlobals.safeSendersTracer == null)
				{
					ExTraceGlobals.safeSendersTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.safeSendersTracer;
			}
		}

		public static Trace BypassCheckTracer
		{
			get
			{
				if (ExTraceGlobals.bypassCheckTracer == null)
				{
					ExTraceGlobals.bypassCheckTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.bypassCheckTracer;
			}
		}

		private static Guid componentGuid = new Guid("175562D6-54D7-4C59-A421-598E03755639");

		private static Trace agentTracer = null;

		private static Trace blockSendersTracer = null;

		private static Trace safeSendersTracer = null;

		private static Trace bypassCheckTracer = null;
	}
}
