using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Write")]
	internal class MoveFolderRequest : CopyOrMoveEntityRequest<Folder>
	{
		public MoveFolderRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new MoveFolderCommand(this);
		}
	}
}
