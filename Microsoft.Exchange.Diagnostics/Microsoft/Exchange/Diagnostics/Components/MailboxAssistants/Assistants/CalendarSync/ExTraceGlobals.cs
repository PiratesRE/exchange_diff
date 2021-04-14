using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.CalendarSync
{
	public static class ExTraceGlobals
	{
		public static Trace CalendarSyncAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.calendarSyncAssistantTracer == null)
				{
					ExTraceGlobals.calendarSyncAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.calendarSyncAssistantTracer;
			}
		}

		private static Guid componentGuid = new Guid("21C31235-EE44-4c3d-9876-9F91AC6F8EAA");

		private static Trace calendarSyncAssistantTracer = null;
	}
}
