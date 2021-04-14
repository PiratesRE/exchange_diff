using System;

namespace Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.Calendar
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

		public static Trace UnexpectedPathTracer
		{
			get
			{
				if (ExTraceGlobals.unexpectedPathTracer == null)
				{
					ExTraceGlobals.unexpectedPathTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.unexpectedPathTracer;
			}
		}

		public static Trace CalendarItemValuesTracer
		{
			get
			{
				if (ExTraceGlobals.calendarItemValuesTracer == null)
				{
					ExTraceGlobals.calendarItemValuesTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.calendarItemValuesTracer;
			}
		}

		public static Trace ProcessingTracer
		{
			get
			{
				if (ExTraceGlobals.processingTracer == null)
				{
					ExTraceGlobals.processingTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.processingTracer;
			}
		}

		public static Trace ProcessingRequestTracer
		{
			get
			{
				if (ExTraceGlobals.processingRequestTracer == null)
				{
					ExTraceGlobals.processingRequestTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.processingRequestTracer;
			}
		}

		public static Trace ProcessingResponseTracer
		{
			get
			{
				if (ExTraceGlobals.processingResponseTracer == null)
				{
					ExTraceGlobals.processingResponseTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.processingResponseTracer;
			}
		}

		public static Trace ProcessingCancellationTracer
		{
			get
			{
				if (ExTraceGlobals.processingCancellationTracer == null)
				{
					ExTraceGlobals.processingCancellationTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.processingCancellationTracer;
			}
		}

		public static Trace CachedStateTracer
		{
			get
			{
				if (ExTraceGlobals.cachedStateTracer == null)
				{
					ExTraceGlobals.cachedStateTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.cachedStateTracer;
			}
		}

		public static Trace OldMessageDeletionTracer
		{
			get
			{
				if (ExTraceGlobals.oldMessageDeletionTracer == null)
				{
					ExTraceGlobals.oldMessageDeletionTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.oldMessageDeletionTracer;
			}
		}

		public static Trace PFDTracer
		{
			get
			{
				if (ExTraceGlobals.pFDTracer == null)
				{
					ExTraceGlobals.pFDTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.pFDTracer;
			}
		}

		public static Trace ProcessingMeetingForwardNotificationTracer
		{
			get
			{
				if (ExTraceGlobals.processingMeetingForwardNotificationTracer == null)
				{
					ExTraceGlobals.processingMeetingForwardNotificationTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.processingMeetingForwardNotificationTracer;
			}
		}

		private static Guid componentGuid = new Guid("57785AFC-670A-4e9e-9AFB-5A6AD9A01AD5");

		private static Trace generalTracer = null;

		private static Trace unexpectedPathTracer = null;

		private static Trace calendarItemValuesTracer = null;

		private static Trace processingTracer = null;

		private static Trace processingRequestTracer = null;

		private static Trace processingResponseTracer = null;

		private static Trace processingCancellationTracer = null;

		private static Trace cachedStateTracer = null;

		private static Trace oldMessageDeletionTracer = null;

		private static Trace pFDTracer = null;

		private static Trace processingMeetingForwardNotificationTracer = null;
	}
}
