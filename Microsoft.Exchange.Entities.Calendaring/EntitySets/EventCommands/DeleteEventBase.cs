using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal abstract class DeleteEventBase : DeleteEntityCommand<Events>
	{
		protected override VoidResult OnExecute()
		{
			Event @event = this.ReadEvent(base.EntityKey);
			if (@event.IsOrganizer)
			{
				this.CancelEvent(base.EntityKey);
			}
			else if (@event.IsCancelled)
			{
				this.DeleteCancelledEventFromAttendeesCalendar(@event);
			}
			else
			{
				RespondToEventParameters parameters = new RespondToEventParameters
				{
					Response = ResponseType.Declined,
					SendResponse = true
				};
				this.RespondToEvent(base.EntityKey, parameters);
			}
			return VoidResult.Value;
		}

		protected virtual Event ReadEvent(string eventId)
		{
			return this.Scope.Read(eventId, this.Context);
		}

		protected virtual void CancelEvent(string eventId)
		{
			this.Scope.Cancel(eventId, null, this.Context);
		}

		protected virtual void RespondToEvent(string eventId, RespondToEventParameters parameters)
		{
			this.Scope.Respond(eventId, parameters, this.Context);
		}

		protected abstract void DeleteCancelledEventFromAttendeesCalendar(Event eventToDelete);
	}
}
