using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class MoveFolderResponse : CopyOrMoveEntityResponse<Folder>
	{
		public MoveFolderResponse(MoveFolderRequest request) : base(request)
		{
		}
	}
}
