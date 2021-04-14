using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission
{
	public static class ExTraceGlobals
	{
		public static Trace StoreDriverSubmissionTracer
		{
			get
			{
				if (ExTraceGlobals.storeDriverSubmissionTracer == null)
				{
					ExTraceGlobals.storeDriverSubmissionTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.storeDriverSubmissionTracer;
			}
		}

		public static Trace MapiStoreDriverSubmissionTracer
		{
			get
			{
				if (ExTraceGlobals.mapiStoreDriverSubmissionTracer == null)
				{
					ExTraceGlobals.mapiStoreDriverSubmissionTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.mapiStoreDriverSubmissionTracer;
			}
		}

		public static Trace MailboxTransportSubmissionServiceTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxTransportSubmissionServiceTracer == null)
				{
					ExTraceGlobals.mailboxTransportSubmissionServiceTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.mailboxTransportSubmissionServiceTracer;
			}
		}

		public static Trace MeetingForwardNotificationTracer
		{
			get
			{
				if (ExTraceGlobals.meetingForwardNotificationTracer == null)
				{
					ExTraceGlobals.meetingForwardNotificationTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.meetingForwardNotificationTracer;
			}
		}

		public static Trace ModeratedTransportTracer
		{
			get
			{
				if (ExTraceGlobals.moderatedTransportTracer == null)
				{
					ExTraceGlobals.moderatedTransportTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.moderatedTransportTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace SubmissionConnectionTracer
		{
			get
			{
				if (ExTraceGlobals.submissionConnectionTracer == null)
				{
					ExTraceGlobals.submissionConnectionTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.submissionConnectionTracer;
			}
		}

		public static Trace SubmissionConnectionPoolTracer
		{
			get
			{
				if (ExTraceGlobals.submissionConnectionPoolTracer == null)
				{
					ExTraceGlobals.submissionConnectionPoolTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.submissionConnectionPoolTracer;
			}
		}

		public static Trace ParkedItemSubmitterAgentTracer
		{
			get
			{
				if (ExTraceGlobals.parkedItemSubmitterAgentTracer == null)
				{
					ExTraceGlobals.parkedItemSubmitterAgentTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.parkedItemSubmitterAgentTracer;
			}
		}

		private static Guid componentGuid = new Guid("2b76aa96-0fe5-4c87-8101-1d236c9fa3ab");

		private static Trace storeDriverSubmissionTracer = null;

		private static Trace mapiStoreDriverSubmissionTracer = null;

		private static Trace mailboxTransportSubmissionServiceTracer = null;

		private static Trace meetingForwardNotificationTracer = null;

		private static Trace moderatedTransportTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace submissionConnectionTracer = null;

		private static Trace submissionConnectionPoolTracer = null;

		private static Trace parkedItemSubmitterAgentTracer = null;
	}
}
