using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.MailboxProcessor
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

		private static Guid componentGuid = new Guid("401BDE63-12DA-45E8-A634-FCE43B2617B6");

		private static Trace generalTracer = null;
	}
}
