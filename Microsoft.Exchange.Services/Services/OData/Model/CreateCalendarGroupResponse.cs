using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateCalendarGroupResponse : CreateEntityResponse<CalendarGroup>
	{
		public CreateCalendarGroupResponse(CreateCalendarGroupRequest request) : base(request)
		{
		}
	}
}
