using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DeleteEvent : DeleteEventBase
	{
		internal IEventCommandFactory CommandFactory { get; set; }

		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.DeleteEventTracer;
			}
		}

		protected override void DeleteCancelledEventFromAttendeesCalendar(Event eventToDelete)
		{
			this.Scope.EventDataProvider.Delete(this.Scope.IdConverter.ToStoreObjectId(eventToDelete.Id), DeleteItemFlags.MoveToDeletedItems);
		}
	}
}
