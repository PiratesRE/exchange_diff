using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct Entities_BirthdayCalendarTags
	{
		public const int CreateBirthdayEventForContact = 0;

		public const int BirthdayCalendarReference = 1;

		public const int BirthdayAssistantBusinessLogic = 2;

		public const int EnableBirthdayCalendar = 3;

		public const int BirthdayCalendars = 4;

		public const int BirthdayEventDataProvider = 5;

		public const int BirthdayContactDataProvider = 6;

		public const int UpdateBirthdaysForLinkedContacts = 7;

		public const int UpdateBirthdayEventForContact = 8;

		public const int DeleteBirthdayEventForContact = 9;

		public const int GetBirthdayCalendarView = 10;

		public static Guid guid = new Guid("F89B9EF1-6D5A-48BF-85C8-445007D8B947");
	}
}
