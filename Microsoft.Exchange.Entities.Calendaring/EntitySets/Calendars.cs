using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.Calendaring.EntitySets.CalendarCommands;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class Calendars : StorageEntitySet<Calendars, Calendar, IMailboxSession>, ICalendars, IEntitySet<Calendar>
	{
		protected Calendars(IStorageEntitySetScope<IMailboxSession> parentScope, CalendarGroupReference calendarGroupForNewCalendars, IEntityCommandFactory<Calendars, Calendar> commandFactory = null) : base(parentScope, "Calendars", commandFactory ?? EntityCommandFactory<Calendars, Calendar, CreateCalendar, DeleteCalendar, FindCalendars, ReadCalendar, UpdateCalendar>.Instance)
		{
			this.CalendarGroupForNewCalendars = calendarGroupForNewCalendars;
		}

		public virtual CalendarFolderDataProvider CalendarFolderDataProvider
		{
			get
			{
				CalendarFolderDataProvider result;
				if ((result = this.calendarFolderDataProvider) == null)
				{
					result = (this.calendarFolderDataProvider = new CalendarFolderDataProvider(this, base.Session.GetDefaultFolderId(DefaultFolderType.Calendar)));
				}
				return result;
			}
		}

		public virtual CalendarGroupEntryDataProvider CalendarGroupEntryDataProvider
		{
			get
			{
				CalendarGroupEntryDataProvider result;
				if ((result = this.calendarGroupEntryDataProvider) == null)
				{
					result = (this.calendarGroupEntryDataProvider = new CalendarGroupEntryDataProvider(this));
				}
				return result;
			}
		}

		public CalendarGroupReference CalendarGroupForNewCalendars { get; private set; }

		public ICalendarReference this[string calendarId]
		{
			get
			{
				return new CalendarReference(this, calendarId);
			}
		}

		public Calendar Create(string name)
		{
			return base.Create(new Calendar
			{
				Name = name
			}, null);
		}

		private CalendarFolderDataProvider calendarFolderDataProvider;

		private CalendarGroupEntryDataProvider calendarGroupEntryDataProvider;
	}
}
