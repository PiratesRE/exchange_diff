using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetContactFolderResponse : GetEntityResponse<ContactFolder>
	{
		public GetContactFolderResponse(GetContactFolderRequest request) : base(request)
		{
		}
	}
}
