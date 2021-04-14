using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.DataProviders;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ReadEvent : ReadEntityCommand<Events, Event>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.ReadEventTracer;
			}
		}

		protected override Event OnExecute()
		{
			StoreId entityStoreId = this.GetEntityStoreId();
			EventDataProvider dataProvider = this.Scope.GetDataProvider(entityStoreId);
			Event @event = dataProvider.Read(entityStoreId);
			this.Scope.TimeAdjuster.AdjustTimeProperties(@event, this.Scope.Session.ExTimeZone);
			return @event;
		}
	}
}
