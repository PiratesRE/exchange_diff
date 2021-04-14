using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UpdateEventResponse : UpdateEntityResponse<Event>
	{
		public UpdateEventResponse(UpdateEventRequest request) : base(request)
		{
		}
	}
}
