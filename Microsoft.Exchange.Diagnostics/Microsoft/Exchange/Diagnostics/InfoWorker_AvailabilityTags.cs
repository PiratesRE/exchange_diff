using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct InfoWorker_AvailabilityTags
	{
		public const int Initialize = 0;

		public const int Security = 1;

		public const int CalendarView = 2;

		public const int Configuration = 5;

		public const int PublicFolderRequest = 7;

		public const int IntraSiteCalendarRequest = 9;

		public const int MeetingSuggestions = 10;

		public const int AutoDiscover = 11;

		public const int MailboxConnectionCache = 13;

		public const int PFD = 14;

		public const int DnsReader = 15;

		public const int Message = 16;

		public const int FaultInjection = 17;

		public static Guid guid = new Guid("A7F9AB97-3B1B-4e10-B58F-E58136B9DA0A");
	}
}
