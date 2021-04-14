using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UpdateCalendarGroupResponse : UpdateEntityResponse<CalendarGroup>
	{
		public UpdateCalendarGroupResponse(UpdateCalendarGroupRequest request) : base(request)
		{
		}
	}
}
