using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets.Commands;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ExpandSeries : KeyedEntityCommand<Events, ExpandedEvent>
	{
		public ExpandEventParameters Parameters { get; set; }

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.ExpandSeriesTracer;
			}
		}

		protected override ExpandedEvent OnExecute()
		{
			IList<Event> list = new List<Event>();
			IList<string> list2 = new List<string>();
			Event @event = this.ReadMaster();
			if (@event.Type != EventType.SeriesMaster)
			{
				throw new InvalidRequestException(CalendaringStrings.ErrorCantExpandSingleItem);
			}
			ExDateTime windowStart = this.Parameters.IsPropertySet(this.Parameters.Schema.WindowStartProperty) ? this.Parameters.WindowStart : ExDateTime.MinValue;
			ExDateTime windowEnd = this.Parameters.IsPropertySet(this.Parameters.Schema.WindowEndProperty) ? this.Parameters.WindowEnd : ExDateTime.MaxValue;
			if (this.Parameters.ReturnExceptions)
			{
				if (@event.PatternedRecurrence != null)
				{
					list = this.GetPatternRecurrenceExceptionsInWindow(@event, windowStart, windowEnd);
				}
				else
				{
					list = this.GetOccurrencesInWindow(@event, windowStart, windowEnd);
				}
			}
			if (this.Parameters.ReturnCancellations)
			{
				list2 = this.GetDeletedOccurrenceIds(@event, windowStart, windowEnd);
			}
			if (@event.PatternedRecurrence != null && this.Parameters.ReturnRegularOccurrences)
			{
				list.AddRange(this.GetPatternRecurrenceOccurrenceInWindow(@event, windowStart, windowEnd));
			}
			return new ExpandedEvent
			{
				RecurrenceMaster = (this.Parameters.ReturnMaster ? @event : null),
				Occurrences = ((this.Parameters.ReturnExceptions || this.Parameters.ReturnRegularOccurrences) ? list : null),
				CancelledOccurrences = (this.Parameters.ReturnCancellations ? list2 : null)
			};
		}

		protected override void UpdateCustomLoggingData()
		{
			base.UpdateCustomLoggingData();
			this.SetCustomLoggingData("ExpandEventParameters", this.Parameters);
		}

		protected virtual Event ReadMaster()
		{
			StoreId id = this.Scope.IdConverter.ToStoreObjectId(base.EntityKey);
			return this.Scope.EventDataProvider.Read(id);
		}

		protected virtual IList<Event> GetOccurrencesInWindow(Event master, ExDateTime windowStart, ExDateTime windowEnd)
		{
			List<Event> occurrencesInWindowFullInfo = new List<Event>();
			this.Scope.EventDataProvider.ForEachSeriesItem(master, delegate(Event instance)
			{
				if (this.IsInWindow(windowStart, windowEnd, instance))
				{
					occurrencesInWindowFullInfo.Add(this.Scope.EventDataProvider.Read(instance.StoreId));
				}
				return true;
			}, new Func<IStorePropertyBag, Event>(this.GetOccurrenceWithStartAndEnd), null, ExpandSeries.SortByStartDateAscending, null, ExpandSeries.PropertiesForInstancesQuery);
			return occurrencesInWindowFullInfo;
		}

		protected virtual IList<Event> GetPatternRecurrenceExceptionsInWindow(Event master, ExDateTime windowStart, ExDateTime windowEnd)
		{
			List<Event> list = new List<Event>();
			using (ICalendarItemBase calendarItemBase = this.Scope.EventDataProvider.BindToStoreObject(IdConverter.Instance.GetStoreId(master)))
			{
				CalendarItem calendarItem = calendarItemBase as CalendarItem;
				if (calendarItem != null && calendarItem.Recurrence != null)
				{
					IList<OccurrenceInfo> modifiedOccurrences = calendarItem.Recurrence.GetModifiedOccurrences();
					foreach (OccurrenceInfo occurrenceInfo in modifiedOccurrences)
					{
						if (CalendarFolder.IsInWindow(windowStart, windowEnd, occurrenceInfo.StartTime, occurrenceInfo.EndTime))
						{
							list.Add(this.Scope.Read(IdConverter.Instance.ToStringId(occurrenceInfo.VersionedId, this.Scope.Session), null));
						}
					}
				}
			}
			return list;
		}

		protected virtual IList<Event> GetPatternRecurrenceOccurrenceInWindow(Event master, ExDateTime windowStart, ExDateTime windowEnd)
		{
			List<Event> list = new List<Event>();
			using (ICalendarItemBase calendarItemBase = this.Scope.EventDataProvider.BindToStoreObject(IdConverter.Instance.GetStoreId(master)))
			{
				CalendarItem calendarItem = calendarItemBase as CalendarItem;
				if (calendarItem != null && calendarItem.Recurrence != null)
				{
					IList<OccurrenceInfo> occurrenceInfoList = calendarItem.Recurrence.GetOccurrenceInfoList(windowStart, windowEnd);
					foreach (OccurrenceInfo occurrenceInfo in occurrenceInfoList)
					{
						Event @event = this.Scope.Read(IdConverter.Instance.ToStringId(occurrenceInfo.VersionedId, this.Scope.Session), null);
						if (@event.Type == EventType.Occurrence)
						{
							list.Add(@event);
						}
					}
				}
			}
			return list;
		}

		protected virtual IList<string> GetDeletedOccurrenceIds(Event master, ExDateTime windowStart, ExDateTime windowEnd)
		{
			List<string> list = new List<string>();
			StoreId storeId = IdConverter.Instance.GetStoreId(master);
			using (ICalendarItemBase calendarItemBase = this.Scope.EventDataProvider.BindToStoreObject(storeId))
			{
				CalendarItem calendarItem = calendarItemBase as CalendarItem;
				if (calendarItem != null && calendarItem.Recurrence != null)
				{
					ExDateTime[] deletedOccurrences = calendarItem.Recurrence.GetDeletedOccurrences();
					foreach (ExDateTime exDateTime in deletedOccurrences)
					{
						ExDateTime endTime = exDateTime.Add(master.End.Subtract(master.Start));
						if (CalendarFolder.IsInWindow(windowStart, windowEnd, exDateTime, endTime))
						{
							OccurrenceStoreObjectId storeId2 = new OccurrenceStoreObjectId(StoreId.GetStoreObjectId(storeId).ProviderLevelItemId, exDateTime);
							list.Add(IdConverter.Instance.ToStringId(storeId2, this.Scope.Session));
						}
					}
				}
			}
			return list;
		}

		protected virtual bool IsInWindow(ExDateTime windowStart, ExDateTime windowEnd, Event instance)
		{
			return CalendarFolder.IsInWindow(windowStart, windowEnd, instance.Start, instance.End);
		}

		private Event GetOccurrenceWithStartAndEnd(IStorePropertyBag propertyBag)
		{
			Event basicSeriesEventData = EventExtensions.GetBasicSeriesEventData(propertyBag, this.Scope);
			basicSeriesEventData.Start = propertyBag.GetValueOrDefault<ExDateTime>(CalendarItemInstanceSchema.StartTime, ExDateTime.MinValue);
			basicSeriesEventData.End = propertyBag.GetValueOrDefault<ExDateTime>(CalendarItemInstanceSchema.EndTime, ExDateTime.MaxValue);
			return basicSeriesEventData;
		}

		private static readonly SortBy SortByStartDateAscending = new SortBy(CalendarItemInstanceSchema.StartTime, SortOrder.Ascending);

		private static readonly PropertyDefinition[] PropertiesForInstancesQuery = new PropertyDefinition[]
		{
			CalendarItemInstanceSchema.StartTime,
			CalendarItemInstanceSchema.EndTime
		};
	}
}
