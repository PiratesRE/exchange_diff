using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets.Commands;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	internal abstract class CreateSeriesInternalBase : EntityCommand<Events, Event>
	{
		internal string ClientId { get; set; }

		internal static IEnumerable<Event> CreateSeriesInstances<T>(Event master, Event masterChange, EventDataProvider eventDataProvider, IDictionary<T, Event> occurrencesAlreadyCreated, Func<IEventInternal, T> selectKey, Action<IEventInternal> prepareOccurrence = null)
		{
			List<Event> list = new List<Event>();
			if (masterChange == null || masterChange.Occurrences == null)
			{
				return list;
			}
			bool flag = occurrencesAlreadyCreated.Count > 0;
			foreach (Event @event in masterChange.Occurrences)
			{
				if (prepareOccurrence != null)
				{
					prepareOccurrence(@event);
				}
				if (!flag || !occurrencesAlreadyCreated.ContainsKey(selectKey(@event)))
				{
					master.CopyMasterPropertiesTo(@event, true, null, true);
					Event item = eventDataProvider.Create(@event);
					list.Add(item);
				}
			}
			list.AddRange(occurrencesAlreadyCreated.Values);
			return list;
		}

		internal Event SendMessagesForSeries(Event masterForInstanceCreation, int seriesSequenceNumber, string occurrencesViewPropertiesBlob)
		{
			if (!masterForInstanceCreation.IsDraft)
			{
				SeriesEventDataProvider seriesEventDataProvider = this.Scope.SeriesEventDataProvider;
				using (ICalendarItemBase calendarItemBase = seriesEventDataProvider.BindToWrite(masterForInstanceCreation.StoreId, masterForInstanceCreation.ChangeKey))
				{
					if (this.ShouldSendMeetingRequest(calendarItemBase))
					{
						calendarItemBase.SendMeetingMessages(true, new int?(seriesSequenceNumber), false, true, occurrencesViewPropertiesBlob, null);
						calendarItemBase.Load();
						return seriesEventDataProvider.ConvertToEntity(calendarItemBase);
					}
				}
				return masterForInstanceCreation;
			}
			return masterForInstanceCreation;
		}

		internal void SendMessagesForInstances(IEnumerable<Event> occurrences, int seriesSequenceNumber, byte[] masterGoid)
		{
			SeriesEventDataProvider seriesEventDataProvider = this.Scope.SeriesEventDataProvider;
			foreach (Event @event in occurrences)
			{
				using (ICalendarItemBase calendarItemBase = seriesEventDataProvider.BindToWrite(@event.StoreId, @event.ChangeKey))
				{
					if (this.ShouldSendMeetingRequest(calendarItemBase))
					{
						calendarItemBase.SendMeetingMessages(true, new int?(seriesSequenceNumber), false, true, null, masterGoid);
					}
				}
			}
		}

		protected abstract int CalculateSeriesCreationHash(Event master);

		protected virtual void ValidateParameters()
		{
			if (string.IsNullOrEmpty(this.ClientId))
			{
				throw new InvalidRequestException(CalendaringStrings.MandatoryParameterClientIdNotSpecified);
			}
		}

		protected virtual Event SendNprMeetingMessages(Event master, IEnumerable<Event> instances)
		{
			Event result = this.SendMessagesForSeries(master, 1, this.Scope.EventDataProvider.CreateOccurrenceViewPropertiesBlob(master));
			byte[] masterGoid = (((IEventInternal)master).GlobalObjectId != null) ? new GlobalObjectId(((IEventInternal)master).GlobalObjectId).Bytes : null;
			this.SendMessagesForInstances(instances, 1, masterGoid);
			return result;
		}

		protected void ValidateCreationHash(Event eventWithKnownHash, Event otherEvent)
		{
			int num = this.CalculateSeriesCreationHash(otherEvent);
			if (num != ((IEventInternal)eventWithKnownHash).SeriesCreationHash)
			{
				throw new ClientIdAlreadyInUseException();
			}
		}

		protected IEnumerable<Event> CreateSeriesInstances(Event master, Event masterChange, Dictionary<int, Event> occurrencesAlreadyCreated, int initialInstanceCreationIndex = 0)
		{
			IEnumerable<Event> result;
			try
			{
				this.Scope.EventDataProvider.BeforeStoreObjectSaved += new Action<Event, ICalendarItemBase>(this.StampRetryProperties);
				result = CreateSeriesInternalBase.CreateSeriesInstances<int>(master, masterChange, this.Scope.EventDataProvider, occurrencesAlreadyCreated, (IEventInternal e) => e.InstanceCreationIndex, delegate(IEventInternal e)
				{
					e.InstanceCreationIndex = initialInstanceCreationIndex++;
				});
			}
			finally
			{
				this.Scope.EventDataProvider.BeforeStoreObjectSaved -= new Action<Event, ICalendarItemBase>(this.StampRetryProperties);
			}
			return result;
		}

		protected Event CreateSeriesMaster(Event master)
		{
			SeriesEventDataProvider seriesEventDataProvider = this.Scope.SeriesEventDataProvider;
			Event result;
			try
			{
				seriesEventDataProvider.BeforeStoreObjectSaved += new Action<Event, ICalendarItemBase>(this.StampRetryProperties);
				this.AdjustMasterParametersForCreation(master);
				Event @event = seriesEventDataProvider.Create(master);
				result = @event;
			}
			finally
			{
				seriesEventDataProvider.BeforeStoreObjectSaved -= new Action<Event, ICalendarItemBase>(this.StampRetryProperties);
			}
			return result;
		}

		protected void AdjustMasterParametersForCreation(Event master)
		{
			((IEventInternal)master).SeriesCreationHash = this.CalculateSeriesCreationHash(master);
			master.Start = ExDateTime.MaxValue;
			master.End = ExDateTime.MinValue;
			master.AdjustSeriesStartAndEndTimes(master.Occurrences);
		}

		protected virtual Event FindSeriesObjects(Event nprMaster, Dictionary<int, Event> occurrencesAlreadyCreated)
		{
			Event masterAlreadyCreated = null;
			this.Scope.EventDataProvider.ForEachSeriesItem(nprMaster, delegate(Event instance)
			{
				occurrencesAlreadyCreated[((IEventInternal)instance).InstanceCreationIndex] = instance;
				return true;
			}, new Func<IStorePropertyBag, Event>(this.GetBasicEventDataForRetry), delegate(Event master)
			{
				masterAlreadyCreated = master;
			}, null, CalendarItemBaseSchema.EventClientId, new PropertyDefinition[]
			{
				CalendarItemBaseSchema.EventClientId,
				CalendarItemSchema.InstanceCreationIndex,
				CalendarItemSeriesSchema.SeriesCreationHash
			});
			return masterAlreadyCreated;
		}

		private Event GetBasicEventDataForRetry(IStorePropertyBag propertyBag)
		{
			Event basicSeriesEventData = EventExtensions.GetBasicSeriesEventData(propertyBag, this.Scope);
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(CalendarItemBaseSchema.EventClientId, null);
			int valueOrDefault2 = propertyBag.GetValueOrDefault<int>(CalendarItemSchema.InstanceCreationIndex, -1);
			int valueOrDefault3 = propertyBag.GetValueOrDefault<int>(CalendarItemSeriesSchema.SeriesCreationHash, -1);
			basicSeriesEventData.ClientId = valueOrDefault;
			IEventInternal eventInternal = basicSeriesEventData;
			if (valueOrDefault2 != -1)
			{
				eventInternal.InstanceCreationIndex = valueOrDefault2;
			}
			if (valueOrDefault3 != -1)
			{
				eventInternal.SeriesCreationHash = valueOrDefault3;
			}
			return basicSeriesEventData;
		}

		private bool ShouldSendMeetingRequest(ICalendarItemBase calendarItemBase)
		{
			return !calendarItemBase.MeetingRequestWasSent && calendarItemBase.AttendeeCollection != null && calendarItemBase.AttendeeCollection.Count > 0;
		}

		private void StampRetryProperties(IEvent sourceEntity, ICalendarItemBase calendarItemBase)
		{
			calendarItemBase.ClientId = this.ClientId;
			ICalendarItem calendarItem = calendarItemBase as ICalendarItem;
			if (calendarItem != null)
			{
				calendarItem.InstanceCreationIndex = ((IEventInternal)sourceEntity).InstanceCreationIndex;
			}
		}
	}
}
