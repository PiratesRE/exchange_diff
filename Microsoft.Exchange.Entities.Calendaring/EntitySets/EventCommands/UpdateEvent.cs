using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class UpdateEvent : UpdateEventBase
	{
		public bool SendMeetingMessagesOnSave
		{
			get
			{
				return this.sendMeetingMessagesOnSave;
			}
			set
			{
				this.sendMeetingMessagesOnSave = value;
			}
		}

		public int? SeriesSequenceNumber { get; set; }

		public byte[] MasterGoid { get; set; }

		public bool PropagationInProgress { get; set; }

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.UpdateEventTracer;
			}
		}

		protected override Event OnExecute()
		{
			EventDataProvider eventDataProvider = this.Scope.EventDataProvider;
			Event result;
			try
			{
				eventDataProvider.StoreObjectSaved += this.OnStoreObjectSaved;
				eventDataProvider.BeforeStoreObjectSaved += this.DataProviderOnBeforeStoreObjectSaved;
				if (!this.PropagationInProgress)
				{
					using (ICalendarItemBase calendarItemBase = eventDataProvider.ValidateAndBindToWrite(base.Entity))
					{
						Event @event = eventDataProvider.ConvertToEntity(calendarItemBase);
						base.MergeAttendeesList(@event.Attendees);
					}
				}
				result = eventDataProvider.Update(base.Entity, this.Context);
			}
			finally
			{
				eventDataProvider.StoreObjectSaved -= this.OnStoreObjectSaved;
				eventDataProvider.BeforeStoreObjectSaved -= this.DataProviderOnBeforeStoreObjectSaved;
			}
			return result;
		}

		protected virtual void DataProviderOnBeforeStoreObjectSaved(Event update, ICalendarItemBase itemToSave)
		{
			this.Scope.TimeAdjuster.AdjustTimeProperties(itemToSave);
		}

		private void OnStoreObjectSaved(object sender, ICalendarItemBase calendarItemBase)
		{
			this.Scope.EventDataProvider.TryLogCalendarEventActivity(ActivityId.UpdateCalendarEvent, calendarItemBase.Id.ObjectId);
			if (this.SendMeetingMessagesOnSave && calendarItemBase.IsOrganizer())
			{
				bool flag;
				CalendarItemAccessors.IsDraft.TryGetValue(calendarItemBase, out flag);
				if (!flag && (calendarItemBase.IsMeeting || (calendarItemBase.AttendeeCollection != null && calendarItemBase.AttendeeCollection.Count > 0)))
				{
					calendarItemBase.SendMeetingMessages(true, this.SeriesSequenceNumber, false, true, null, this.MasterGoid);
					calendarItemBase.Load();
				}
			}
		}

		private bool sendMeetingMessagesOnSave = true;
	}
}
