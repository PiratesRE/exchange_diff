using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class RespondToEventCommand : EntityContainersCommand<RespondToEventRequest, RespondToEventResponse>
	{
		public RespondToEventCommand(RespondToEventRequest request) : base(request)
		{
		}

		protected override RespondToEventResponse InternalExecute()
		{
			IEvents events = base.GetCalendarContainer(null).Events;
			events.Respond(EwsIdConverter.ODataIdToEwsId(base.Request.Id), base.Request.RespondToEventParameters, base.CreateCommandContext(null));
			return new RespondToEventResponse(base.Request);
		}
	}
}
