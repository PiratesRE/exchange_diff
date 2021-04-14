using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Contacts.Write")]
	[AllowedOAuthGrant("Contacts.Read")]
	internal class GetContactFolderRequest : GetEntityRequest<ContactFolder>
	{
		public GetContactFolderRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new GetContactFolderCommand(this);
		}
	}
}
