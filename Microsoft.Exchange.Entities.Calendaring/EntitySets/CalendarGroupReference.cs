using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	internal class CalendarGroupReference : StorageEntityReference<CalendarGroups, CalendarGroup, IMailboxSession>, ICalendarGroupReference, IEntityReference<CalendarGroup>
	{
		public CalendarGroupReference(CalendarGroups calendarGroups, string calendarGroupKey) : base(calendarGroups, calendarGroupKey)
		{
		}

		public CalendarGroupReference(CalendarGroups calendarGroups, StoreId calendarGroupStoreId) : base(calendarGroups, calendarGroupStoreId)
		{
		}

		public CalendarGroupReference(CalendarGroups calendarGroups, CalendarGroupType calendarGroupType) : base(calendarGroups)
		{
			this.calendarGroupType = new CalendarGroupType?(calendarGroupType);
		}

		public ICalendars Calendars
		{
			get
			{
				ICalendars result;
				if ((result = this.calendars) == null)
				{
					result = (this.calendars = new CalendarsInCalendarGroup(this));
				}
				return result;
			}
		}

		protected override StoreId ResolveReference()
		{
			StoreId id;
			using (ICalendarGroup calendarGroup = base.XsoFactory.BindToCalendarGroup(base.StoreSession, this.calendarGroupType.Value))
			{
				id = calendarGroup.Id;
			}
			return id;
		}

		protected override string GetRelativeDescription()
		{
			if (this.calendarGroupType != null)
			{
				return '.' + this.calendarGroupType.Value.ToString();
			}
			return base.GetRelativeDescription();
		}

		private CalendarGroupType? calendarGroupType;

		private ICalendars calendars;
	}
}
