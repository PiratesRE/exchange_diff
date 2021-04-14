using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindFoldersResponse : FindEntitiesResponse<Folder>
	{
		public FindFoldersResponse(FindFoldersRequest request) : base(request)
		{
		}
	}
}
