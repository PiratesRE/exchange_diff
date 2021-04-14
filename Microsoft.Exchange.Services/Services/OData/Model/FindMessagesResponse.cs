using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindMessagesResponse : FindEntitiesResponse<Message>
	{
		public FindMessagesResponse(FindMessagesRequest request) : base(request)
		{
		}
	}
}
