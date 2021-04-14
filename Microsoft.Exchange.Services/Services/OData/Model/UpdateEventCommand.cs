using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UpdateEventCommand : EntityContainersCommand<UpdateEventRequest, UpdateEventResponse>
	{
		public UpdateEventCommand(UpdateEventRequest request) : base(request)
		{
		}

		protected override UpdateEventResponse InternalExecute()
		{
			Event entity = DataEntityObjectFactory.CreateAndSetPropertiesOnDataEntityForUpdate<Event>(base.Request.Change);
			IEvents events = base.GetCalendarContainer(null).Events;
			Event dataEntityEvent = events.Update(EwsIdConverter.ODataIdToEwsId(base.Request.Id), entity, base.CreateCommandContext(null));
			return new UpdateEventResponse(base.Request)
			{
				Result = GetEventCommand.DataEntityEventToEntity(dataEntityEvent, base.Request.ODataQueryOptions, base.ExchangeService)
			};
		}
	}
}
