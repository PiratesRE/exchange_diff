using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Read")]
	[AllowedOAuthGrant("Calendars.Read")]
	[AllowedOAuthGrant("Contacts.Read")]
	[AllowedOAuthGrant("Mail.Write")]
	[AllowedOAuthGrant("Mail.Send")]
	[AllowedOAuthGrant("Calendars.Write")]
	[AllowedOAuthGrant("user_impersonation")]
	[AllowedOAuthGrant("Contacts.Write")]
	internal class GetUserRequest : GetEntityRequest<User>
	{
		public GetUserRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new GetUserCommand(this);
		}
	}
}
