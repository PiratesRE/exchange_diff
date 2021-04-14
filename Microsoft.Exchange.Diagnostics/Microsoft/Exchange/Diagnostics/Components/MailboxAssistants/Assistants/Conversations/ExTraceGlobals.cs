using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.Conversations
{
	public static class ExTraceGlobals
	{
		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		private static Guid componentGuid = new Guid("85EA4EE7-EDB2-4A00-93D7-764949EF1225");

		private static Trace generalTracer = null;
	}
}
