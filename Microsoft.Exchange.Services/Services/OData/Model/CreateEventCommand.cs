using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateEventCommand : EntityContainersCommand<CreateEventRequest, CreateEventResponse>
	{
		public CreateEventCommand(CreateEventRequest request) : base(request)
		{
		}

		protected override CreateEventResponse InternalExecute()
		{
			Event entity = DataEntityObjectFactory.CreateAndSetPropertiesOnDataEntityForCreate<Event>(base.Request.Template);
			IEvents events = base.GetCalendarContainer(base.Request.CalendarId).Events;
			Event dataEntityEvent = events.Create(entity, base.CreateCommandContext(null));
			return new CreateEventResponse(base.Request)
			{
				Result = GetEventCommand.DataEntityEventToEntity(dataEntityEvent, base.Request.ODataQueryOptions, base.ExchangeService)
			};
		}
	}
}
