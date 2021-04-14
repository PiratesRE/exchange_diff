using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.Serialization;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	internal class CreateNewSeries : CreateSeriesInternalBase
	{
		internal Event Entity { get; set; }

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.CreateSeriesTracer;
			}
		}

		protected override Event OnExecute()
		{
			this.ValidateParameters();
			Dictionary<int, Event> occurrencesAlreadyCreated = new Dictionary<int, Event>();
			Event @event = this.FindSeriesObjects(this.Entity, occurrencesAlreadyCreated);
			Event event2;
			if (@event != null)
			{
				base.ValidateCreationHash(@event, this.Entity);
				event2 = this.Scope.SeriesEventDataProvider.Read(this.Scope.IdConverter.GetStoreId(@event));
			}
			else
			{
				event2 = base.CreateSeriesMaster(this.Entity);
			}
			IEnumerable<Event> instances = base.CreateSeriesInstances(event2, this.Entity, occurrencesAlreadyCreated, 0);
			if (event2.IsDraft)
			{
				return event2;
			}
			return this.SendNprMeetingMessages(event2, instances);
		}

		protected override void ValidateParameters()
		{
			base.ValidateParameters();
			if (this.Entity.IsPropertySet(this.Entity.Schema.SeriesIdProperty))
			{
				throw new InvalidRequestException(CalendaringStrings.ErrorCallerCantSpecifySeriesId);
			}
			if (this.Entity.Occurrences == null || this.Entity.Occurrences.Count == 0)
			{
				throw new InvalidRequestException(CalendaringStrings.ErrorOccurrencesListRequired);
			}
			if ((long)this.Entity.Occurrences.Count > 50L)
			{
				throw new InvalidRequestException(ServerStrings.ExTooManyInstancesOnSeries(50U));
			}
		}

		protected override int CalculateSeriesCreationHash(Event master)
		{
			string s = EntitySerializer.Serialize<Event>(master);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			return (int)ComputeCRC.Compute(0U, bytes, 0, bytes.Length);
		}
	}
}
