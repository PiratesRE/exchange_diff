using System;
using System.Collections.Generic;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring
{
	public interface IEvents : IEntitySet<Event>
	{
		IEventReference this[string eventId]
		{
			get;
		}

		void Cancel(string key, CancelEventParameters parameters, CommandContext context = null);

		void Forward(string key, ForwardEventParameters parameters, CommandContext context = null);

		ExpandedEvent Expand(string key, ExpandEventParameters parameters, CommandContext context = null);

		IEnumerable<Event> GetCalendarView(ICalendarViewParameters parameters, CommandContext context = null);

		void Respond(string key, RespondToEventParameters parameters, CommandContext context = null);

		Event Update(string key, Event entity, UpdateEventParameters updateEventParameters, CommandContext context = null);

		Event ConvertSingleEventToNprSeries(string key, IList<Event> additionalInstancesToAdd, string clientId, CommandContext context = null);
	}
}
