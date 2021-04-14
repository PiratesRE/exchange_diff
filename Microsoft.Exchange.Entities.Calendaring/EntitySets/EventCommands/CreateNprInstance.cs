using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.ReliableActions;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CreateNprInstance : CreateSingleEventBase
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.CreateOccurrenceTracer;
			}
		}

		protected override void ValidateSeriesId()
		{
			if (!base.Entity.IsPropertySet(base.Entity.Schema.SeriesIdProperty))
			{
				throw new InvalidRequestException(CalendaringStrings.ErrorCallerMustSpecifySeriesId);
			}
		}

		protected override void ValidateParameters()
		{
			base.ValidateParameters();
			if (!base.Entity.IsPropertySet(base.Entity.Schema.ClientIdProperty))
			{
				throw new InvalidRequestException(CalendaringStrings.MandatoryParameterClientIdNotSpecified);
			}
		}

		protected override Event CreateNewEvent()
		{
			Event master = new Event
			{
				SeriesId = base.Entity.SeriesId,
				Occurrences = new List<Event>(),
				Type = EventType.SeriesMaster
			};
			this.Scope.EventDataProvider.ForEachSeriesItem(master, delegate(Event instance)
			{
				master.Occurrences.Add(instance);
				return true;
			}, (IStorePropertyBag propertyBag) => CreateNprInstance.GetBasicSeriesEventDataWithClinetId(propertyBag, this.Scope), delegate(Event series)
			{
				master.Id = series.Id;
				master.ChangeKey = series.ChangeKey;
				master.ClientId = series.ClientId;
			}, null, null, new PropertyDefinition[]
			{
				CalendarItemBaseSchema.EventClientId
			});
			if (!master.IsPropertySet(master.Schema.IdProperty))
			{
				throw new SeriesNotFoundException(base.Entity.SeriesId);
			}
			int count = master.Occurrences.Count;
			if ((long)(count + 1) > 50L)
			{
				throw new InvalidOperationException(ServerStrings.ExTooManyInstancesOnSeries(50U));
			}
			if (string.Equals(master.ClientId, base.Entity.ClientId))
			{
				throw new ClientIdAlreadyInUseException();
			}
			Event @event = this.Scope.SeriesEventDataProvider.Read(this.Scope.IdConverter.ToStoreObjectId(master.Id));
			if (@event.IsPropertySet(@event.Schema.LastExecutedInteropActionProperty))
			{
				((IActionPropagationState)base.Entity).LastExecutedAction = ((IActionPropagationState)@event).LastExecutedAction;
			}
			if (@event.AdjustSeriesStartAndEndTimes(new Event[]
			{
				base.Entity
			}))
			{
				this.Scope.SeriesEventDataProvider.Update(@event, this.Context);
			}
			Event event2 = master.Occurrences.FirstOrDefault((Event occurrence) => string.Equals(occurrence.ClientId, base.Entity.ClientId));
			if (event2 == null)
			{
				event2 = CreateNprInstance.CreateCopyOccurrenceWithStartAndEnd(base.Entity, @event);
				try
				{
					this.Scope.EventDataProvider.BeforeStoreObjectSaved += new Action<Event, ICalendarItemBase>(this.StampRetryProperties);
					event2 = this.Scope.EventDataProvider.Create(event2);
				}
				finally
				{
					this.Scope.EventDataProvider.BeforeStoreObjectSaved -= new Action<Event, ICalendarItemBase>(this.StampRetryProperties);
				}
			}
			base.Entity.Id = event2.Id;
			if (!base.Entity.IsPropertySet(base.Entity.Schema.IsDraftProperty))
			{
				base.Entity.IsDraft = @event.IsDraft;
			}
			return this.Scope.EventDataProvider.Update(base.Entity, this.Context);
		}

		private static Event GetBasicSeriesEventDataWithClinetId(IStorePropertyBag propertyBag, IStorageEntitySetScope<IStoreSession> scope)
		{
			Event basicSeriesEventData = EventExtensions.GetBasicSeriesEventData(propertyBag, scope);
			basicSeriesEventData.ClientId = propertyBag.GetValueOrDefault<string>(CalendarItemBaseSchema.EventClientId, null);
			return basicSeriesEventData;
		}

		private static Event CreateCopyOccurrenceWithStartAndEnd(Event instance, Event masterFromStore)
		{
			Event @event = new Event
			{
				Start = instance.Start,
				End = instance.End,
				Type = EventType.Exception
			};
			masterFromStore.CopyMasterPropertiesTo(@event, true, null, true);
			@event.IsDraft = true;
			@event.ClientId = instance.ClientId;
			return @event;
		}

		private void StampRetryProperties(IEvent sourceEntity, ICalendarItemBase calendarItemBase)
		{
			calendarItemBase.ClientId = base.Entity.ClientId;
		}
	}
}
