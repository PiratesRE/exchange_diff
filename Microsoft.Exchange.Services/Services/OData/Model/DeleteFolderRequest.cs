using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Write")]
	internal class DeleteFolderRequest : DeleteEntityRequest<Folder>
	{
		public DeleteFolderRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new DeleteFolderCommand(this);
		}
	}
}
