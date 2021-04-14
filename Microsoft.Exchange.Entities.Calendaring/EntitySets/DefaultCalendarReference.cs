using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	internal class DefaultCalendarReference : CalendarReference
	{
		public DefaultCalendarReference(MailboxCalendars calendars) : base(calendars)
		{
		}

		protected override string GetRelativeDescription()
		{
			return ".Default";
		}

		protected override StoreId ResolveCalendarFolderId()
		{
			return base.EntitySet.StoreSession.GetDefaultFolderId(DefaultFolderType.Calendar);
		}

		protected override StoreId ResolveReference()
		{
			StoreId calendarId;
			using (ICalendarGroupEntry calendarGroupEntry = base.XsoFactory.BindToCalendarGroupEntry(base.StoreSession, base.GetCalendarFolderId()))
			{
				calendarId = calendarGroupEntry.CalendarId;
			}
			return calendarId;
		}
	}
}
