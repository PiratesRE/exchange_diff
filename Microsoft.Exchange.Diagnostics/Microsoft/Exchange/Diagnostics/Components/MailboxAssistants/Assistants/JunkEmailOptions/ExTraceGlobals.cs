using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.JunkEmailOptions
{
	public static class ExTraceGlobals
	{
		public static Trace JEOAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.jEOAssistantTracer == null)
				{
					ExTraceGlobals.jEOAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.jEOAssistantTracer;
			}
		}

		private static Guid componentGuid = new Guid("F0C5D2BC-B9E3-4095-B124-2AF4940121D1");

		private static Trace jEOAssistantTracer = null;
	}
}
