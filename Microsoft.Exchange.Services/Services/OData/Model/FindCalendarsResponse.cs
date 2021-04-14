using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindCalendarsResponse : FindEntitiesResponse<Calendar>
	{
		public FindCalendarsResponse(FindCalendarsRequest request) : base(request)
		{
		}
	}
}
