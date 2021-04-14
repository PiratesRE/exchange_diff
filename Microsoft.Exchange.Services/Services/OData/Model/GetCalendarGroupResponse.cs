using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetCalendarGroupResponse : GetEntityResponse<CalendarGroup>
	{
		public GetCalendarGroupResponse(GetCalendarGroupRequest request) : base(request)
		{
		}
	}
}
