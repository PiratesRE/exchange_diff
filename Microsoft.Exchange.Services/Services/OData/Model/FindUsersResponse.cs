using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindUsersResponse : FindEntitiesResponse<User>
	{
		public FindUsersResponse(FindUsersRequest request) : base(request)
		{
		}
	}
}
