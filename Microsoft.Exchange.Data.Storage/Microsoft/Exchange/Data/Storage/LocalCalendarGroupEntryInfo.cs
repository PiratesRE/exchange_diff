using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LocalCalendarGroupEntryInfo : CalendarGroupEntryInfo
	{
		public bool IsInternetCalendar { get; private set; }

		public LocalCalendarGroupEntryInfo(string calendarName, VersionedId id, LegacyCalendarColor color, StoreObjectId calendarId, byte[] calendarOrdinal, Guid parentGroupId, bool isICal, ExDateTime lastModifiedTime) : base(calendarName, id, color, calendarId, parentGroupId, calendarOrdinal, lastModifiedTime)
		{
			this.IsInternetCalendar = isICal;
		}
	}
}
