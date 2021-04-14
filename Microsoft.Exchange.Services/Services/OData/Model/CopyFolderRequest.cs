using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Write")]
	internal class CopyFolderRequest : CopyOrMoveEntityRequest<Folder>
	{
		public CopyFolderRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new CopyFolderCommand(this);
		}
	}
}
