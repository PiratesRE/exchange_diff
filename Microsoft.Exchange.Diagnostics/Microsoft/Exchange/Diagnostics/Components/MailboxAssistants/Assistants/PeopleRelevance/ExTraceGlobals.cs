using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.PeopleRelevance
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

		private static Guid componentGuid = new Guid("30440839-91AA-4107-9C09-6FA7DFFFCF39");

		private static Trace generalTracer = null;
	}
}
