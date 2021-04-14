using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.Reminders
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

		public static Trace HeuristicsTracer
		{
			get
			{
				if (ExTraceGlobals.heuristicsTracer == null)
				{
					ExTraceGlobals.heuristicsTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.heuristicsTracer;
			}
		}

		private static Guid componentGuid = new Guid("2723E866-2665-4E65-A02E-4AD0BDDC3C5A");

		private static Trace generalTracer = null;

		private static Trace heuristicsTracer = null;
	}
}
