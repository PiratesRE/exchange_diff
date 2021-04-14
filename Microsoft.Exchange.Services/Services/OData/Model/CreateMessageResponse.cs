using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateMessageResponse : CreateEntityResponse<Message>
	{
		public CreateMessageResponse(CreateMessageRequest request) : base(request)
		{
		}
	}
}
