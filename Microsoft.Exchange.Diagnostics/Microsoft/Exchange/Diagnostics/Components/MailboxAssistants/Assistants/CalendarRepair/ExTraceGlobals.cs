using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.CalendarRepair
{
	public static class ExTraceGlobals
	{
		public static Trace CalendarRepairAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.calendarRepairAssistantTracer == null)
				{
					ExTraceGlobals.calendarRepairAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.calendarRepairAssistantTracer;
			}
		}

		public static Trace PFDTracer
		{
			get
			{
				if (ExTraceGlobals.pFDTracer == null)
				{
					ExTraceGlobals.pFDTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.pFDTracer;
			}
		}

		private static Guid componentGuid = new Guid("80005C8C-8262-4BB9-B6DA-F288D9C542E4");

		private static Trace calendarRepairAssistantTracer = null;

		private static Trace pFDTracer = null;
	}
}
