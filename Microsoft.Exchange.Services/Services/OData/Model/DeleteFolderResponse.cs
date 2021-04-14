using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DeleteFolderResponse : DeleteEntityResponse<Folder>
	{
		public DeleteFolderResponse(DeleteFolderRequest request) : base(request)
		{
		}
	}
}
