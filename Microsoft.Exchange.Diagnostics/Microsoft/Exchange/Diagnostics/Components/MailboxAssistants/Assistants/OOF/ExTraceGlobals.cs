using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.OOF
{
	public static class ExTraceGlobals
	{
		public static Trace MailboxDataTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxDataTracer == null)
				{
					ExTraceGlobals.mailboxDataTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.mailboxDataTracer;
			}
		}

		public static Trace OofSettingsHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.oofSettingsHandlerTracer == null)
				{
					ExTraceGlobals.oofSettingsHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.oofSettingsHandlerTracer;
			}
		}

		public static Trace LegacyOofReplyTemplateHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.legacyOofReplyTemplateHandlerTracer == null)
				{
					ExTraceGlobals.legacyOofReplyTemplateHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.legacyOofReplyTemplateHandlerTracer;
			}
		}

		public static Trace LegacyOofStateHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.legacyOofStateHandlerTracer == null)
				{
					ExTraceGlobals.legacyOofStateHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.legacyOofStateHandlerTracer;
			}
		}

		public static Trace OofSchedulerTracer
		{
			get
			{
				if (ExTraceGlobals.oofSchedulerTracer == null)
				{
					ExTraceGlobals.oofSchedulerTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.oofSchedulerTracer;
			}
		}

		public static Trace OofScheduledActionTracer
		{
			get
			{
				if (ExTraceGlobals.oofScheduledActionTracer == null)
				{
					ExTraceGlobals.oofScheduledActionTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.oofScheduledActionTracer;
			}
		}

		public static Trace PFDTracer
		{
			get
			{
				if (ExTraceGlobals.pFDTracer == null)
				{
					ExTraceGlobals.pFDTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.pFDTracer;
			}
		}

		public static Trace OofScheduleStoreTracer
		{
			get
			{
				if (ExTraceGlobals.oofScheduleStoreTracer == null)
				{
					ExTraceGlobals.oofScheduleStoreTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.oofScheduleStoreTracer;
			}
		}

		private static Guid componentGuid = new Guid("EF72E07E-3E86-4250-9083-C18CD673631D");

		private static Trace mailboxDataTracer = null;

		private static Trace oofSettingsHandlerTracer = null;

		private static Trace legacyOofReplyTemplateHandlerTracer = null;

		private static Trace legacyOofStateHandlerTracer = null;

		private static Trace oofSchedulerTracer = null;

		private static Trace oofScheduledActionTracer = null;

		private static Trace pFDTracer = null;

		private static Trace oofScheduleStoreTracer = null;
	}
}
