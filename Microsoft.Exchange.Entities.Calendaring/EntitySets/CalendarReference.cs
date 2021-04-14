using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	internal class CalendarReference : StorageEntityReference<Calendars, Calendar, IMailboxSession>, ILocalCalendarReference, ICalendarReference, IEntityReference<Calendar>
	{
		public CalendarReference(Calendars calendars, string calendarId) : base(calendars, calendarId)
		{
		}

		protected CalendarReference(Calendars calendars) : base(calendars)
		{
		}

		public IEvents Events
		{
			get
			{
				return new Events(this, this);
			}
		}

		public StoreId GetCalendarFolderId()
		{
			if (this.calendarFolderId == null)
			{
				this.calendarFolderId = this.ResolveCalendarFolderId();
			}
			return this.calendarFolderId;
		}

		protected virtual StoreId ResolveCalendarFolderId()
		{
			StoreId storeId = base.GetStoreId();
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(storeId);
			if (storeObjectId.IsFolderId)
			{
				return storeId;
			}
			StoreId calendarId;
			using (ICalendarGroupEntry calendarGroupEntry = base.XsoFactory.BindToCalendarGroupEntry(base.StoreSession, storeObjectId))
			{
				calendarId = calendarGroupEntry.CalendarId;
			}
			return calendarId;
		}

		private StoreId calendarFolderId;
	}
}
