using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindContactFoldersResponse : FindEntitiesResponse<ContactFolder>
	{
		public FindContactFoldersResponse(FindContactFoldersRequest request) : base(request)
		{
		}
	}
}
