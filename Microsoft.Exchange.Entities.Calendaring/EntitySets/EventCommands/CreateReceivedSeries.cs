using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CreateReceivedSeries : CreateEntityCommand<Events, Event>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.CreateReceivedSeriesTracer;
			}
		}

		protected override Event OnExecute()
		{
			this.ValidateParameters();
			this.CreateSeriesInstances();
			return base.Entity;
		}

		protected void ValidateParameters()
		{
			if (!base.Entity.IsPropertySet(base.Entity.Schema.SeriesIdProperty))
			{
				throw new ArgumentException("SeriesId should be populated from meeting message", "SeriesId");
			}
			if (base.Entity.Occurrences != null)
			{
				foreach (Event @event in base.Entity.Occurrences)
				{
					ArgumentValidator.ThrowIfNull("occurrence", @event);
					ArgumentValidator.ThrowIfNullOrEmpty("GOID", ((IEventInternal)@event).GlobalObjectId);
					ArgumentValidator.ThrowIfNull("Start", @event.Start);
					ArgumentValidator.ThrowIfNull("End", @event.End);
				}
			}
		}

		private IDictionary<string, Event> FindOccurences()
		{
			Dictionary<string, Event> occurrencesAlreadyCreated = new Dictionary<string, Event>();
			this.Scope.EventDataProvider.ForEachSeriesItem(base.Entity, delegate(Event instance)
			{
				occurrencesAlreadyCreated[((IEventInternal)instance).GlobalObjectId] = instance;
				return true;
			}, new Func<IStorePropertyBag, Event>(this.GetBasicEventData), null, null, null, new PropertyDefinition[]
			{
				CalendarItemBaseSchema.GlobalObjectId
			});
			return occurrencesAlreadyCreated;
		}

		private Event GetBasicEventData(IStorePropertyBag propertyBag)
		{
			Event basicSeriesEventData = EventExtensions.GetBasicSeriesEventData(propertyBag, this.Scope);
			byte[] valueOrDefault = propertyBag.GetValueOrDefault<byte[]>(CalendarItemBaseSchema.GlobalObjectId, null);
			if (valueOrDefault != null)
			{
				IEventInternal eventInternal = basicSeriesEventData;
				eventInternal.GlobalObjectId = new GlobalObjectId(valueOrDefault).ToString();
			}
			return basicSeriesEventData;
		}

		private void CreateSeriesInstances()
		{
			IDictionary<string, Event> occurrencesAlreadyCreated = this.FindOccurences();
			CreateSeriesInternalBase.CreateSeriesInstances<string>(base.Entity, base.Entity, this.Scope.EventDataProvider, occurrencesAlreadyCreated, (IEventInternal e) => e.GlobalObjectId, null);
		}
	}
}
