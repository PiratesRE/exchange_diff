using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct Entities_CalendaringTags
	{
		public const int EventDataProvider = 0;

		public const int ReadEvent = 1;

		public const int CreateEvent = 2;

		public const int UpdateEvent = 3;

		public const int DeleteEvent = 4;

		public const int FindEvents = 5;

		public const int CalendarDataProvider = 6;

		public const int ReadCalendar = 7;

		public const int CreateCalendar = 8;

		public const int UpdateCalendar = 9;

		public const int DeleteCalendar = 10;

		public const int FindCalendars = 11;

		public const int CancelEvent = 12;

		public const int RespondToEvent = 13;

		public const int InstancesQuery = 14;

		public const int CalendarInterop = 15;

		public const int CreateSeries = 16;

		public const int CancelSeries = 17;

		public const int UpdateSeries = 18;

		public const int SeriesPendingActionsInterop = 19;

		public const int SeriesInlineInterop = 20;

		public const int CreateOccurrence = 21;

		public const int ReadCalendarGroup = 22;

		public const int CreateCalendarGroup = 23;

		public const int UpdateCalendarGroup = 24;

		public const int DeleteCalendarGroup = 25;

		public const int FindCalendarGroups = 26;

		public const int MeetingMessageProcessing = 27;

		public const int CreateReceivedSeries = 28;

		public const int RespondToSeries = 29;

		public const int ForwardEvent = 30;

		public const int ForwardSeries = 31;

		public const int SeriesActionParser = 32;

		public const int ExpandSeries = 33;

		public const int MeetingRequestMessageDataProvider = 34;

		public const int RespondToMeetingRequest = 35;

		public const int ConvertSingleEventToNprSeries = 36;

		public const int GetCalendarView = 37;

		public const int DeleteSeries = 38;

		public static Guid guid = new Guid("6B844120-1AE2-4E8C-ABDB-F3D7F3E95388");
	}
}
