using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class CalendarGroupEntryInfo : FolderTreeDataInfo
	{
		public string CalendarName { get; private set; }

		public StoreObjectId CalendarId { get; private set; }

		public LegacyCalendarColor LegacyCalendarColor { get; private set; }

		public CalendarColor CalendarColor { get; private set; }

		public Guid ParentGroupClassId { get; private set; }

		public CalendarGroupEntryInfo(string calendarName, VersionedId id, LegacyCalendarColor color, StoreObjectId calendarId, Guid parentGroupId, byte[] calendarOrdinal, ExDateTime lastModifiedTime) : base(id, calendarOrdinal, lastModifiedTime)
		{
			Util.ThrowOnNullArgument(calendarName, "calendarName");
			EnumValidator.ThrowIfInvalid<LegacyCalendarColor>(color, "color");
			this.CalendarId = calendarId;
			this.CalendarName = calendarName;
			this.LegacyCalendarColor = color;
			this.ParentGroupClassId = parentGroupId;
			this.CalendarColor = LegacyCalendarColorConverter.FromLegacyCalendarColor(color);
		}
	}
}
