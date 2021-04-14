using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetUserResponse : GetEntityResponse<User>
	{
		public GetUserResponse(GetUserRequest request) : base(request)
		{
		}
	}
}
