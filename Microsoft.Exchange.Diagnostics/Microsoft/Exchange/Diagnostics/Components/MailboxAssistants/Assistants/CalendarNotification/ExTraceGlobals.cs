using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.CalendarNotification
{
	public static class ExTraceGlobals
	{
		public static Trace AssistantTracer
		{
			get
			{
				if (ExTraceGlobals.assistantTracer == null)
				{
					ExTraceGlobals.assistantTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.assistantTracer;
			}
		}

		public static Trace SystemMailboxTracer
		{
			get
			{
				if (ExTraceGlobals.systemMailboxTracer == null)
				{
					ExTraceGlobals.systemMailboxTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.systemMailboxTracer;
			}
		}

		public static Trace UserSettingsTracer
		{
			get
			{
				if (ExTraceGlobals.userSettingsTracer == null)
				{
					ExTraceGlobals.userSettingsTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.userSettingsTracer;
			}
		}

		private static Guid componentGuid = new Guid("B6075A29-F2B0-4B58-8791-8869F07F732A");

		private static Trace assistantTracer = null;

		private static Trace systemMailboxTracer = null;

		private static Trace userSettingsTracer = null;
	}
}
