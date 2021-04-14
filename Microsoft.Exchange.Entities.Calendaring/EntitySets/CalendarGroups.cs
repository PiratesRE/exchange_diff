using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.Calendaring.EntitySets.CalendarGroupCommands;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CalendarGroups : StorageEntitySet<CalendarGroups, CalendarGroup, IMailboxSession>, ICalendarGroups, IEntitySet<CalendarGroup>
	{
		protected internal CalendarGroups(IStorageEntitySetScope<IMailboxSession> parentScope, IEntityCommandFactory<CalendarGroups, CalendarGroup> commandFactory = null) : base(parentScope, "CalendarGroups", commandFactory ?? EntityCommandFactory<CalendarGroups, CalendarGroup, CreateCalendarGroup, DeleteCalendarGroup, FindCalendarGroups, ReadCalendarGroup, UpdateCalendarGroup>.Instance)
		{
		}

		public CalendarGroupReference MyCalendars
		{
			get
			{
				CalendarGroupReference result;
				if ((result = this.myCalendars) == null)
				{
					result = (this.myCalendars = new CalendarGroupReference(this, CalendarGroupType.MyCalendars));
				}
				return result;
			}
		}

		public CalendarGroupReference OtherCalendars
		{
			get
			{
				CalendarGroupReference result;
				if ((result = this.otherCalendars) == null)
				{
					result = (this.otherCalendars = new CalendarGroupReference(this, CalendarGroupType.OtherCalendars));
				}
				return result;
			}
		}

		ICalendarGroupReference ICalendarGroups.MyCalendars
		{
			get
			{
				return this.MyCalendars;
			}
		}

		ICalendarGroupReference ICalendarGroups.OtherCalendars
		{
			get
			{
				return this.OtherCalendars;
			}
		}

		internal virtual CalendarGroupDataProvider CalendarGroupDataProvider
		{
			get
			{
				CalendarGroupDataProvider result;
				if ((result = this.calendarGroupDataProvider) == null)
				{
					result = (this.calendarGroupDataProvider = new CalendarGroupDataProvider(this));
				}
				return result;
			}
		}

		public CalendarGroupReference this[string calendarGroupId]
		{
			get
			{
				return new CalendarGroupReference(this, calendarGroupId);
			}
		}

		ICalendarGroupReference ICalendarGroups.this[string calendarGroupId]
		{
			get
			{
				return this[calendarGroupId];
			}
		}

		public CalendarGroup Create(string name)
		{
			return base.Create(new CalendarGroup
			{
				Name = name
			}, null);
		}

		private CalendarGroupReference myCalendars;

		private CalendarGroupReference otherCalendars;

		private CalendarGroupDataProvider calendarGroupDataProvider;
	}
}
