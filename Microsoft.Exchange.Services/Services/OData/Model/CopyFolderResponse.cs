using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CopyFolderResponse : CopyOrMoveEntityResponse<Folder>
	{
		public CopyFolderResponse(CopyFolderRequest request) : base(request)
		{
		}
	}
}
