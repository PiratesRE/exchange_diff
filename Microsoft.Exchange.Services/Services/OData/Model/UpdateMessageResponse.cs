using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UpdateMessageResponse : UpdateEntityResponse<Message>
	{
		public UpdateMessageResponse(UpdateMessageRequest request) : base(request)
		{
		}
	}
}
