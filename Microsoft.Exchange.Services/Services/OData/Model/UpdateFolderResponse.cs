using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UpdateFolderResponse : UpdateEntityResponse<Folder>
	{
		public UpdateFolderResponse(UpdateFolderRequest request) : base(request)
		{
		}
	}
}
