using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct StoreDriverTags
	{
		public const int StoreDriver = 0;

		public const int MapiSubmit = 1;

		public const int MapiDeliver = 2;

		public const int MailSubmissionService = 3;

		public const int BridgeheadPicker = 4;

		public const int CalendarProcessing = 5;

		public const int ExceptionHandling = 6;

		public const int MeetingForwardNotification = 7;

		public const int ApprovalAgent = 8;

		public const int MailboxRule = 9;

		public const int ModeratedTransport = 10;

		public const int Conversations = 12;

		public const int MailSubmissionRedundancyManager = 14;

		public const int UMPlayonPhoneAgent = 15;

		public const int SmsDeliveryAgent = 16;

		public const int UMPartnerMessageAgent = 17;

		public const int FaultInjection = 18;

		public const int SubmissionConnection = 19;

		public const int SubmissionConnectionPool = 20;

		public const int UnJournalDeliveryAgent = 21;

		public static Guid guid = new Guid("a77be922-83fd-4eb1-9e88-6caadbc7340e");
	}
}
