using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DeleteCalendarCommand : EntityContainersCommand<DeleteCalendarRequest, DeleteCalendarResponse>
	{
		public DeleteCalendarCommand(DeleteCalendarRequest request) : base(request)
		{
		}

		protected override DeleteCalendarResponse InternalExecute()
		{
			string key = EwsIdConverter.ODataIdToEwsId(base.Request.Id);
			this.EntityContainers.Calendaring.Calendars.Delete(key, base.CreateCommandContext(null));
			return new DeleteCalendarResponse(base.Request);
		}
	}
}
