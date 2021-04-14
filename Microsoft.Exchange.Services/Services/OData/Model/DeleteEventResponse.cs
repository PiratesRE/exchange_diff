using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DeleteEventResponse : DeleteEntityResponse<Event>
	{
		public DeleteEventResponse(DeleteEventRequest request) : base(request)
		{
		}
	}
}
