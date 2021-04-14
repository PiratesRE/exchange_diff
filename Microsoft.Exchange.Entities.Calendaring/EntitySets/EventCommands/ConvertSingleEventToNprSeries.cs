using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConvertSingleEventToNprSeries : KeyedEntityCommand<Events, Event>
	{
		internal IList<Event> AdditionalInstancesToAdd { get; set; }

		internal string ClientId { get; set; }

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.ConvertSingleEventToNprSeriesTracer;
			}
		}

		protected override Event OnExecute()
		{
			CreateSeriesFromExistingSingleEvent createSeriesFromExistingSingleEvent = new CreateSeriesFromExistingSingleEvent
			{
				SingleEventId = base.EntityKey,
				AdditionalInstancesToAdd = this.AdditionalInstancesToAdd,
				ClientId = this.ClientId,
				Scope = this.Scope
			};
			return createSeriesFromExistingSingleEvent.Execute(this.Context);
		}
	}
}
