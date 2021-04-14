using System;

namespace Microsoft.Exchange.Diagnostics.Components.ProtocolFilter
{
	public static class ExTraceGlobals
	{
		public static Trace SenderFilterAgentTracer
		{
			get
			{
				if (ExTraceGlobals.senderFilterAgentTracer == null)
				{
					ExTraceGlobals.senderFilterAgentTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.senderFilterAgentTracer;
			}
		}

		public static Trace RecipientFilterAgentTracer
		{
			get
			{
				if (ExTraceGlobals.recipientFilterAgentTracer == null)
				{
					ExTraceGlobals.recipientFilterAgentTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.recipientFilterAgentTracer;
			}
		}

		public static Trace OtherTracer
		{
			get
			{
				if (ExTraceGlobals.otherTracer == null)
				{
					ExTraceGlobals.otherTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.otherTracer;
			}
		}

		private static Guid componentGuid = new Guid("0C5B72B3-290E-4c06-BE9D-D4727DF5FC0D");

		private static Trace senderFilterAgentTracer = null;

		private static Trace recipientFilterAgentTracer = null;

		private static Trace otherTracer = null;
	}
}
