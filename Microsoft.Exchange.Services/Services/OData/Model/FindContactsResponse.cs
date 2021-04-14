using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindContactsResponse : FindEntitiesResponse<Contact>
	{
		public FindContactsResponse(FindContactsRequest request) : base(request)
		{
		}
	}
}
