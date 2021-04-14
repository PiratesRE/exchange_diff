using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DeleteCalendarGroupResponse : DeleteEntityResponse<CalendarGroup>
	{
		public DeleteCalendarGroupResponse(DeleteCalendarGroupRequest request) : base(request)
		{
		}
	}
}
