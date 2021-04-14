using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Write")]
	[AllowedOAuthGrant("Mail.Read")]
	internal class GetMessageRequest : GetEntityRequest<Message>
	{
		public GetMessageRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new GetMessageCommand(this);
		}
	}
}
