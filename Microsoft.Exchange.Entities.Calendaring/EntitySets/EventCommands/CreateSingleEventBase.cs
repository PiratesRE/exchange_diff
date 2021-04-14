using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class CreateSingleEventBase : CreateEventBase
	{
		protected override Event OnExecute()
		{
			Event result;
			try
			{
				this.Scope.EventDataProvider.StoreObjectSaved += this.OnStoreObjectSaved;
				this.ValidateParameters();
				Event @event = this.CreateNewEvent();
				result = @event;
			}
			finally
			{
				this.Scope.EventDataProvider.StoreObjectSaved -= this.OnStoreObjectSaved;
			}
			return result;
		}

		protected abstract Event CreateNewEvent();

		private void OnStoreObjectSaved(object sender, ICalendarItemBase calendarItemBase)
		{
			this.Scope.EventDataProvider.TryLogCalendarEventActivity(ActivityId.CreateCalendarEvent, calendarItemBase.Id.ObjectId);
			bool flag;
			CalendarItemAccessors.IsDraft.TryGetValue(calendarItemBase, out flag);
			if (!flag && calendarItemBase.AttendeeCollection != null && calendarItemBase.AttendeeCollection.Count > 0)
			{
				calendarItemBase.SendMeetingMessages(true, null, false, true, null, null);
				calendarItemBase.Load();
			}
		}
	}
}
