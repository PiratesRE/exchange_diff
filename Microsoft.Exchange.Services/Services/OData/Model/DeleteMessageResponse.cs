using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DeleteMessageResponse : DeleteEntityResponse<Message>
	{
		public DeleteMessageResponse(DeleteMessageRequest request) : base(request)
		{
		}
	}
}
