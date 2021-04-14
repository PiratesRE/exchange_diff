using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LinkedCalendarGroupEntryInfo : CalendarGroupEntryInfo
	{
		public string CalendarOwner { get; private set; }

		public bool IsGeneralScheduleCalendar { get; private set; }

		public bool IsPublicCalendarFolder { get; private set; }

		public LinkedCalendarGroupEntryInfo(string calendarName, VersionedId id, LegacyCalendarColor color, StoreObjectId calendarId, string calendarOwner, Guid parentGroupId, byte[] calendarOrdinal, bool isGeneralScheduleCalendar, bool isPublicCalendarFolder, ExDateTime lastModifiedTime) : base(calendarName, id, color, calendarId, parentGroupId, calendarOrdinal, lastModifiedTime)
		{
			this.CalendarOwner = calendarOwner;
			this.IsGeneralScheduleCalendar = isGeneralScheduleCalendar;
			this.IsPublicCalendarFolder = isPublicCalendarFolder;
		}
	}
}
