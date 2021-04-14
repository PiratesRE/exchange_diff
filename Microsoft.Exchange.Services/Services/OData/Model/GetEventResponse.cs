using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetEventResponse : GetEntityResponse<Event>
	{
		public GetEventResponse(GetEventRequest request) : base(request)
		{
		}
	}
}
