using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Write")]
	internal class UpdateMessageRequest : UpdateEntityRequest<Message>
	{
		public UpdateMessageRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new UpdateMessageCommand(this);
		}
	}
}
