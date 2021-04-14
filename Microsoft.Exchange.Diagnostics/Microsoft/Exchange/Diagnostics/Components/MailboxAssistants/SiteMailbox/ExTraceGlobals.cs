using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.SiteMailbox
{
	public static class ExTraceGlobals
	{
		public static Trace AssistantTracer
		{
			get
			{
				if (ExTraceGlobals.assistantTracer == null)
				{
					ExTraceGlobals.assistantTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.assistantTracer;
			}
		}

		private static Guid componentGuid = new Guid("aefa8e76-9e79-425e-83a7-85e60ee2970a");

		private static Trace assistantTracer = null;
	}
}
