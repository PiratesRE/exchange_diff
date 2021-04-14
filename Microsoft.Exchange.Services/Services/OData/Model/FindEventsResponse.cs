using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindEventsResponse : FindEntitiesResponse<Event>
	{
		public FindEventsResponse(FindEventsRequest request) : base(request)
		{
		}
	}
}
