using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.SharePointSignalStore
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

		private static Guid componentGuid = new Guid("2bb0e55b-1550-4d7e-896d-febef4aaf74c");

		private static Trace generalTracer = null;
	}
}
