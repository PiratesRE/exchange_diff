using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindCalendarGroupsResponse : FindEntitiesResponse<CalendarGroup>
	{
		public FindCalendarGroupsResponse(FindCalendarGroupsRequest request) : base(request)
		{
		}
	}
}
