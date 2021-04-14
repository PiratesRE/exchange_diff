using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Contacts.Read")]
	[AllowedOAuthGrant("Contacts.Write")]
	internal class GetContactRequest : GetEntityRequest<Contact>
	{
		public GetContactRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new GetContactCommand(this);
		}
	}
}
