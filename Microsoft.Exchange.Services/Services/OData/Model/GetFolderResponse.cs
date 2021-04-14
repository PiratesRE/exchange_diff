using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetFolderResponse : ODataResponse<Folder>
	{
		public GetFolderResponse(GetFolderRequest request) : base(request)
		{
		}
	}
}
