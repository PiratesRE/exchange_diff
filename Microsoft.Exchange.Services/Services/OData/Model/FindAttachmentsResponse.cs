using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindAttachmentsResponse : FindEntitiesResponse<Attachment>
	{
		public FindAttachmentsResponse(FindAttachmentsRequest request) : base(request)
		{
		}
	}
}
