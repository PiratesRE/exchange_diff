using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.SharingFolder
{
	public static class ExTraceGlobals
	{
		public static Trace IsEventInterestingTracer
		{
			get
			{
				if (ExTraceGlobals.isEventInterestingTracer == null)
				{
					ExTraceGlobals.isEventInterestingTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.isEventInterestingTracer;
			}
		}

		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		private static Guid componentGuid = new Guid("B506E288-FC0D-4bf2-A8E7-ED0F43DC468F");

		private static Trace isEventInterestingTracer = null;

		private static Trace generalTracer = null;
	}
}
