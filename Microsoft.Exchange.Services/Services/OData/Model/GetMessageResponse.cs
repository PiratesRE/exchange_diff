using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetMessageResponse : GetEntityResponse<Message>
	{
		public GetMessageResponse(GetMessageRequest request) : base(request)
		{
		}
	}
}
