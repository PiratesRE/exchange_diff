using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Send")]
	internal class SendMessageRequest : EntityActionRequest<Message>
	{
		public SendMessageRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new SendMessageCommand(this);
		}
	}
}
