using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.Dar
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

		private static Guid componentGuid = new Guid("5790DD8E-22E8-451E-BCA4-0AF9F5C69A4E");

		private static Trace generalTracer = null;
	}
}
