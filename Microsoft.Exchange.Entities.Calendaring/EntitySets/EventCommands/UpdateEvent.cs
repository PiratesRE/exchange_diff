using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.Calendaring.Interop;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.ReliableActions;
using Microsoft.Exchange.Entities.DataProviders;

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
						if (this.IsNprInstance(calendarItemBase))
						{
							this.ValidateSeriesMasterId(calendarItemBase as ICalendarItem);
							this.PreProcessNprInstance(@event);
						}
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

		protected virtual void PreProcessNprInstance(Event instanceFromStore)
		{
			Event master = this.ReadMasterFromInstance(instanceFromStore);
			if (this.IsPropagationPending(master, instanceFromStore))
			{
				this.ForcePropagation(master, instanceFromStore);
			}
		}

		protected virtual void ForcePropagation(Event master, Event instance)
		{
			PropagateToInstance propagateToInstance = this.CreatePropagateToInstanceCommand(master, instance);
			propagateToInstance.Execute(this.Context);
		}

		protected virtual PropagateToInstance CreatePropagateToInstanceCommand(Event master, Event instance)
		{
			return new PropagateToInstance(CalendarInteropLog.Default, null)
			{
				Entity = master,
				Instance = instance
			};
		}

		protected virtual Event ReadMasterFromInstance(Event instance)
		{
			Event result;
			if (!this.TryReadMasterFromSeriesMasterId(instance, out result))
			{
				ICalendarItemBase storeObject = this.Scope.EventDataProvider.BindToMasterFromSeriesId(instance.SeriesId);
				result = this.Scope.EventDataProvider.ConvertToEntity(storeObject);
			}
			return result;
		}

		protected virtual bool IsPropagationPending(IActionPropagationState master, IActionPropagationState instance)
		{
			Guid? lastExecutedAction = master.LastExecutedAction;
			if (lastExecutedAction == null)
			{
				return false;
			}
			Guid? lastExecutedAction2 = instance.LastExecutedAction;
			return lastExecutedAction2 == null || !lastExecutedAction.Value.Equals(lastExecutedAction2.Value);
		}

		protected virtual void ValidateSeriesMasterId(ICalendarItem storeObject)
		{
			string b;
			bool flag = this.TryGetSeriesMasterId(storeObject, out b);
			if (base.Entity.IsPropertySet(base.Entity.Schema.SeriesMasterIdProperty) && (!flag || base.Entity.SeriesMasterId != b))
			{
				throw new InvalidRequestException(CalendaringStrings.ErrorCallerCantChangeSeriesMasterId);
			}
		}

		protected virtual bool TryGetSeriesMasterId(ICalendarItem storeObject, out string seriesMasterId)
		{
			return CalendarItemAccessors.SeriesMasterId.TryGetValue(storeObject, out seriesMasterId);
		}

		private bool TryReadMasterFromSeriesMasterId(Event instance, out Event master)
		{
			master = null;
			string seriesMasterId = instance.SeriesMasterId;
			if (string.IsNullOrEmpty(seriesMasterId))
			{
				return false;
			}
			bool result;
			try
			{
				StoreId id = this.Scope.IdConverter.ToStoreObjectId(seriesMasterId);
				master = this.Scope.EventDataProvider.Read(id);
				result = true;
			}
			catch (ObjectNotFoundException arg)
			{
				this.Trace.TraceError<ObjectNotFoundException>((long)this.GetHashCode(), "Error while reading master based on SeriesMasterId. {0}", arg);
				result = false;
			}
			return result;
		}

		private bool IsNprInstance(ICalendarItemBase targetStoreObject)
		{
			return !string.IsNullOrEmpty(targetStoreObject.SeriesId) && !ObjectClass.IsCalendarItemSeries(targetStoreObject.ItemClass);
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
