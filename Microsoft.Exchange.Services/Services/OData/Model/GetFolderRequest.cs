using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Write")]
	[AllowedOAuthGrant("Mail.Read")]
	internal class GetFolderRequest : GetEntityRequest<Folder>
	{
		public GetFolderRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new GetFolderCommand(this);
		}
	}
}
