using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DeleteCalendarGroupCommand : EntityContainersCommand<DeleteCalendarGroupRequest, DeleteCalendarGroupResponse>
	{
		public DeleteCalendarGroupCommand(DeleteCalendarGroupRequest request) : base(request)
		{
		}

		protected override DeleteCalendarGroupResponse InternalExecute()
		{
			string key = EwsIdConverter.ODataIdToEwsId(base.Request.Id);
			this.EntityContainers.Calendaring.CalendarGroups.Delete(key, base.CreateCommandContext(null));
			return new DeleteCalendarGroupResponse(base.Request);
		}
	}
}
