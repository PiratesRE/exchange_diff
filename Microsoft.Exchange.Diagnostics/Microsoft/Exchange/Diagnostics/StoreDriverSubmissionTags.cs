using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct StoreDriverSubmissionTags
	{
		public const int StoreDriverSubmission = 0;

		public const int MapiStoreDriverSubmission = 1;

		public const int MailboxTransportSubmissionService = 3;

		public const int MeetingForwardNotification = 7;

		public const int ModeratedTransport = 10;

		public const int FaultInjection = 18;

		public const int SubmissionConnection = 19;

		public const int SubmissionConnectionPool = 20;

		public const int ParkedItemSubmitterAgent = 21;

		public static Guid guid = new Guid("2b76aa96-0fe5-4c87-8101-1d236c9fa3ab");
	}
}
