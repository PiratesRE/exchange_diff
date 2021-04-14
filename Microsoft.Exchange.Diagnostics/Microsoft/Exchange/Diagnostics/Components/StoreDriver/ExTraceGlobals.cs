using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.StoreDriver
{
	public static class ExTraceGlobals
	{
		public static Trace StoreDriverTracer
		{
			get
			{
				if (ExTraceGlobals.storeDriverTracer == null)
				{
					ExTraceGlobals.storeDriverTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.storeDriverTracer;
			}
		}

		public static Trace MapiSubmitTracer
		{
			get
			{
				if (ExTraceGlobals.mapiSubmitTracer == null)
				{
					ExTraceGlobals.mapiSubmitTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.mapiSubmitTracer;
			}
		}

		public static Trace MapiDeliverTracer
		{
			get
			{
				if (ExTraceGlobals.mapiDeliverTracer == null)
				{
					ExTraceGlobals.mapiDeliverTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.mapiDeliverTracer;
			}
		}

		public static Trace MailSubmissionServiceTracer
		{
			get
			{
				if (ExTraceGlobals.mailSubmissionServiceTracer == null)
				{
					ExTraceGlobals.mailSubmissionServiceTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.mailSubmissionServiceTracer;
			}
		}

		public static Trace BridgeheadPickerTracer
		{
			get
			{
				if (ExTraceGlobals.bridgeheadPickerTracer == null)
				{
					ExTraceGlobals.bridgeheadPickerTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.bridgeheadPickerTracer;
			}
		}

		public static Trace CalendarProcessingTracer
		{
			get
			{
				if (ExTraceGlobals.calendarProcessingTracer == null)
				{
					ExTraceGlobals.calendarProcessingTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.calendarProcessingTracer;
			}
		}

		public static Trace ExceptionHandlingTracer
		{
			get
			{
				if (ExTraceGlobals.exceptionHandlingTracer == null)
				{
					ExTraceGlobals.exceptionHandlingTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.exceptionHandlingTracer;
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

		public static Trace ApprovalAgentTracer
		{
			get
			{
				if (ExTraceGlobals.approvalAgentTracer == null)
				{
					ExTraceGlobals.approvalAgentTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.approvalAgentTracer;
			}
		}

		public static Trace MailboxRuleTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxRuleTracer == null)
				{
					ExTraceGlobals.mailboxRuleTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.mailboxRuleTracer;
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

		public static Trace ConversationsTracer
		{
			get
			{
				if (ExTraceGlobals.conversationsTracer == null)
				{
					ExTraceGlobals.conversationsTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.conversationsTracer;
			}
		}

		public static Trace MailSubmissionRedundancyManagerTracer
		{
			get
			{
				if (ExTraceGlobals.mailSubmissionRedundancyManagerTracer == null)
				{
					ExTraceGlobals.mailSubmissionRedundancyManagerTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.mailSubmissionRedundancyManagerTracer;
			}
		}

		public static Trace UMPlayonPhoneAgentTracer
		{
			get
			{
				if (ExTraceGlobals.uMPlayonPhoneAgentTracer == null)
				{
					ExTraceGlobals.uMPlayonPhoneAgentTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.uMPlayonPhoneAgentTracer;
			}
		}

		public static Trace SmsDeliveryAgentTracer
		{
			get
			{
				if (ExTraceGlobals.smsDeliveryAgentTracer == null)
				{
					ExTraceGlobals.smsDeliveryAgentTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.smsDeliveryAgentTracer;
			}
		}

		public static Trace UMPartnerMessageAgentTracer
		{
			get
			{
				if (ExTraceGlobals.uMPartnerMessageAgentTracer == null)
				{
					ExTraceGlobals.uMPartnerMessageAgentTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.uMPartnerMessageAgentTracer;
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

		public static Trace UnJournalDeliveryAgentTracer
		{
			get
			{
				if (ExTraceGlobals.unJournalDeliveryAgentTracer == null)
				{
					ExTraceGlobals.unJournalDeliveryAgentTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.unJournalDeliveryAgentTracer;
			}
		}

		private static Guid componentGuid = new Guid("a77be922-83fd-4eb1-9e88-6caadbc7340e");

		private static Trace storeDriverTracer = null;

		private static Trace mapiSubmitTracer = null;

		private static Trace mapiDeliverTracer = null;

		private static Trace mailSubmissionServiceTracer = null;

		private static Trace bridgeheadPickerTracer = null;

		private static Trace calendarProcessingTracer = null;

		private static Trace exceptionHandlingTracer = null;

		private static Trace meetingForwardNotificationTracer = null;

		private static Trace approvalAgentTracer = null;

		private static Trace mailboxRuleTracer = null;

		private static Trace moderatedTransportTracer = null;

		private static Trace conversationsTracer = null;

		private static Trace mailSubmissionRedundancyManagerTracer = null;

		private static Trace uMPlayonPhoneAgentTracer = null;

		private static Trace smsDeliveryAgentTracer = null;

		private static Trace uMPartnerMessageAgentTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace submissionConnectionTracer = null;

		private static Trace submissionConnectionPoolTracer = null;

		private static Trace unJournalDeliveryAgentTracer = null;
	}
}
