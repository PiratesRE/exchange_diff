using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MailboxAssistants_Assistants_CalendarTags
	{
		public const int General = 0;

		public const int UnexpectedPath = 1;

		public const int CalendarItemValues = 2;

		public const int Processing = 3;

		public const int ProcessingRequest = 4;

		public const int ProcessingResponse = 5;

		public const int ProcessingCancellation = 6;

		public const int CachedState = 7;

		public const int OldMessageDeletion = 8;

		public const int PFD = 9;

		public const int ProcessingMeetingForwardNotification = 10;

		public static Guid guid = new Guid("57785AFC-670A-4e9e-9AFB-5A6AD9A01AD5");
	}
}
