using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Write")]
	internal class UpdateFolderRequest : UpdateEntityRequest<Folder>
	{
		public UpdateFolderRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new UpdateFolderCommand(this);
		}
	}
}
