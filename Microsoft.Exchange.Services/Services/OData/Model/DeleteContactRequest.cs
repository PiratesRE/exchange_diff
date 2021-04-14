using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Contacts.Write")]
	internal class DeleteContactRequest : DeleteEntityRequest<Contact>
	{
		public DeleteContactRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new DeleteContactCommand(this);
		}
	}
}
