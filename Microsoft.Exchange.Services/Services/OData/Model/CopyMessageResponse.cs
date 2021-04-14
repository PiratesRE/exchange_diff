using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CopyMessageResponse : CopyOrMoveEntityResponse<Message>
	{
		public CopyMessageResponse(CopyMessageRequest request) : base(request)
		{
		}
	}
}
