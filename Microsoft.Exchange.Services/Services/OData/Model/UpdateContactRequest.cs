using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Contacts.Write")]
	internal class UpdateContactRequest : UpdateEntityRequest<Contact>
	{
		public UpdateContactRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new UpdateContactCommand(this);
		}
	}
}
