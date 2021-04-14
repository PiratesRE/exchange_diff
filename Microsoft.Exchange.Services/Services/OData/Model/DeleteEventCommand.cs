using System;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DeleteEventCommand : EntityContainersCommand<DeleteEventRequest, DeleteEventResponse>
	{
		public DeleteEventCommand(DeleteEventRequest request) : base(request)
		{
		}

		protected override DeleteEventResponse InternalExecute()
		{
			IEvents events = base.GetCalendarContainer(null).Events;
			events.Delete(EwsIdConverter.ODataIdToEwsId(base.Request.Id), base.CreateCommandContext(null));
			return new DeleteEventResponse(base.Request);
		}
	}
}
