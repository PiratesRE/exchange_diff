using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct StoreDriverDeliveryTags
	{
		public const int StoreDriverDelivery = 0;

		public const int MapiDeliver = 2;

		public const int BridgeheadPicker = 4;

		public const int CalendarProcessing = 5;

		public const int ExceptionHandling = 6;

		public const int MeetingForwardNotification = 7;

		public const int ApprovalAgent = 8;

		public const int MailboxRule = 9;

		public const int ModeratedTransport = 10;

		public const int Conversations = 12;

		public const int UMPlayonPhoneAgent = 15;

		public const int SmsDeliveryAgent = 16;

		public const int UMPartnerMessageAgent = 17;

		public const int FaultInjection = 18;

		public const int GroupEscalationAgent = 22;

		public const int MeetingMessageProcessingAgent = 23;

		public const int MeetingSeriesMessageOrderingAgent = 24;

		public const int SharedMailboxSentItemsAgent = 26;

		public static Guid guid = new Guid("D81003EF-1A7B-4AF0-BA18-236DB5A83114");
	}
}
