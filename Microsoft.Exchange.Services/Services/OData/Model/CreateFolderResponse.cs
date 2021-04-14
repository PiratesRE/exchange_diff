using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateFolderResponse : CreateEntityResponse<Folder>
	{
		public CreateFolderResponse(CreateFolderRequest request) : base(request)
		{
		}
	}
}
