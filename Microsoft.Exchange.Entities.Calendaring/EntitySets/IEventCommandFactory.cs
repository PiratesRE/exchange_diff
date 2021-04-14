using System;
using Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.EntitySets;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets
{
	internal interface IEventCommandFactory : IEntityCommandFactory<Events, Event>
	{
		CancelEventBase CreateCancelCommand(string key, Events scope);

		ConvertSingleEventToNprSeries CreateConvertSingleEventToNprCommand(string key, Events scope);

		ExpandSeries CreateExpandCommand(string key, Events scope);

		ForwardEventBase CreateForwardCommand(string key, Events scope);

		GetCalendarView CreateGetCalendarViewCommand(ICalendarViewParameters parameters, Events scope);

		RespondToEventBase CreateRespondToCommand(string key, Events scope);

		UpdateEventBase CreateUpdateCommand(string key, Event calEvent, Events scope, UpdateEventParameters updateEventParameters);
	}
}
