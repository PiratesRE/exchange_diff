using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants
{
	public static class ExTraceGlobals
	{
		public static Trace AssistantBaseTracer
		{
			get
			{
				if (ExTraceGlobals.assistantBaseTracer == null)
				{
					ExTraceGlobals.assistantBaseTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.assistantBaseTracer;
			}
		}

		public static Trace ExchangeServerTracer
		{
			get
			{
				if (ExTraceGlobals.exchangeServerTracer == null)
				{
					ExTraceGlobals.exchangeServerTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.exchangeServerTracer;
			}
		}

		public static Trace PFDTracer
		{
			get
			{
				if (ExTraceGlobals.pFDTracer == null)
				{
					ExTraceGlobals.pFDTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.pFDTracer;
			}
		}

		public static Trace ProvisioningAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.provisioningAssistantTracer == null)
				{
					ExTraceGlobals.provisioningAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.provisioningAssistantTracer;
			}
		}

		private static Guid componentGuid = new Guid("1A6DC046-AE90-4c5a-AEEA-2CB240A1A620");

		private static Trace assistantBaseTracer = null;

		private static Trace exchangeServerTracer = null;

		private static Trace pFDTracer = null;

		private static Trace provisioningAssistantTracer = null;
	}
}
