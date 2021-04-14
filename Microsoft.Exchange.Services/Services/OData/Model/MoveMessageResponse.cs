using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class MoveMessageResponse : CopyOrMoveEntityResponse<Message>
	{
		public MoveMessageResponse(MoveMessageRequest request) : base(request)
		{
		}
	}
}
