using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.PublicFolder
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

		private static Guid componentGuid = new Guid("376A5667-2C9B-4A3C-AC51-6C504750183E");

		private static Trace assistantTracer = null;
	}
}
