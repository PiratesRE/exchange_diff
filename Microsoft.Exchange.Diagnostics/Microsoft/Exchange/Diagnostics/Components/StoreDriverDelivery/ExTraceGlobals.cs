using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery
{
	public static class ExTraceGlobals
	{
		public static Trace StoreDriverDeliveryTracer
		{
			get
			{
				if (ExTraceGlobals.storeDriverDeliveryTracer == null)
				{
					ExTraceGlobals.storeDriverDeliveryTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.storeDriverDeliveryTracer;
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

		public static Trace GroupEscalationAgentTracer
		{
			get
			{
				if (ExTraceGlobals.groupEscalationAgentTracer == null)
				{
					ExTraceGlobals.groupEscalationAgentTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.groupEscalationAgentTracer;
			}
		}

		public static Trace MeetingMessageProcessingAgentTracer
		{
			get
			{
				if (ExTraceGlobals.meetingMessageProcessingAgentTracer == null)
				{
					ExTraceGlobals.meetingMessageProcessingAgentTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.meetingMessageProcessingAgentTracer;
			}
		}

		public static Trace MeetingSeriesMessageOrderingAgentTracer
		{
			get
			{
				if (ExTraceGlobals.meetingSeriesMessageOrderingAgentTracer == null)
				{
					ExTraceGlobals.meetingSeriesMessageOrderingAgentTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.meetingSeriesMessageOrderingAgentTracer;
			}
		}

		public static Trace SharedMailboxSentItemsAgentTracer
		{
			get
			{
				if (ExTraceGlobals.sharedMailboxSentItemsAgentTracer == null)
				{
					ExTraceGlobals.sharedMailboxSentItemsAgentTracer = new Trace(ExTraceGlobals.componentGuid, 26);
				}
				return ExTraceGlobals.sharedMailboxSentItemsAgentTracer;
			}
		}

		private static Guid componentGuid = new Guid("D81003EF-1A7B-4AF0-BA18-236DB5A83114");

		private static Trace storeDriverDeliveryTracer = null;

		private static Trace mapiDeliverTracer = null;

		private static Trace bridgeheadPickerTracer = null;

		private static Trace calendarProcessingTracer = null;

		private static Trace exceptionHandlingTracer = null;

		private static Trace meetingForwardNotificationTracer = null;

		private static Trace approvalAgentTracer = null;

		private static Trace mailboxRuleTracer = null;

		private static Trace moderatedTransportTracer = null;

		private static Trace conversationsTracer = null;

		private static Trace uMPlayonPhoneAgentTracer = null;

		private static Trace smsDeliveryAgentTracer = null;

		private static Trace uMPartnerMessageAgentTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace groupEscalationAgentTracer = null;

		private static Trace meetingMessageProcessingAgentTracer = null;

		private static Trace meetingSeriesMessageOrderingAgentTracer = null;

		private static Trace sharedMailboxSentItemsAgentTracer = null;
	}
}
