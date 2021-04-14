using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.Search
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

		private static Guid componentGuid = new Guid("48B0B47F-5559-44A4-ACAE-7B55C77BBB2D");

		private static Trace generalTracer = null;
	}
}
