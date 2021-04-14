using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.Serialization;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	internal class CreateSeriesFromExistingSingleEvent : CreateSeriesInternalBase
	{
		internal IList<Event> AdditionalInstancesToAdd { get; set; }

		internal string SingleEventId { get; set; }

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.ConvertSingleEventToNprSeriesTracer;
			}
		}

		protected override Event OnExecute()
		{
			this.ValidateParameters();
			Event @event = this.Scope.Read(this.SingleEventId, this.Context);
			Event event2 = this.ConstructMasterFromSingleEvent(@event);
			Dictionary<int, Event> dictionary = new Dictionary<int, Event>();
			Event event3 = this.FindSeriesObjects(event2, dictionary);
			if (event3 != null)
			{
				base.ValidateCreationHash(event3, event2);
				event2 = this.Scope.Read(event3.Id, null);
			}
			else
			{
				event2 = base.CreateSeriesMaster(event2);
			}
			if (dictionary.Values.FirstOrDefault((Event e) => e.Id.Equals(this.SingleEventId)) == null)
			{
				this.StampSeriesPropertiesOnSingleEvent(@event, event2);
			}
			event2.Occurrences = this.AdditionalInstancesToAdd;
			IEnumerable<Event> second = base.CreateSeriesInstances(event2, event2, dictionary, 1);
			if (@event.IsDraft)
			{
				return event2;
			}
			return this.SendNprMeetingMessages(event2, new List<Event>
			{
				@event
			}.Concat(second));
		}

		protected override int CalculateSeriesCreationHash(Event master)
		{
			List<byte> list = new List<byte>();
			string s = EntitySerializer.Serialize<IList<Event>>(this.AdditionalInstancesToAdd);
			list.AddRange(Encoding.UTF8.GetBytes(s));
			list.AddRange(Encoding.UTF8.GetBytes(this.SingleEventId));
			byte[] array = list.ToArray();
			return (int)ComputeCRC.Compute(0U, array, 0, array.Count<byte>());
		}

		private Event ConstructMasterFromSingleEvent(Event singleEvent)
		{
			Event @event = new Event();
			@event.MergeMasterAndInstanceProperties(singleEvent, true, new Func<Event, Event, PropertyDefinition, bool>(EventExtensions.ShouldCopyProperty));
			@event.Type = EventType.SeriesMaster;
			@event.ClientId = base.ClientId;
			@event.Occurrences = new List<Event>
			{
				singleEvent
			}.Concat(this.AdditionalInstancesToAdd).ToList<Event>();
			return @event;
		}

		private void StampSeriesPropertiesOnSingleEvent(Event singleEvent, Event master)
		{
			singleEvent.SeriesId = master.SeriesId;
			singleEvent.SeriesMasterId = master.Id;
			((IEventInternal)singleEvent).InstanceCreationIndex = 0;
			this.Scope.EventDataProvider.Update(singleEvent, this.Context);
		}
	}
}
