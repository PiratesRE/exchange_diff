using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateEventResponse : CreateEntityResponse<Event>
	{
		public CreateEventResponse(CreateEventRequest request) : base(request)
		{
		}
	}
}
