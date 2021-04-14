using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Write")]
	internal class DeleteMessageRequest : DeleteEntityRequest<Message>
	{
		public DeleteMessageRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new DeleteMessageCommand(this);
		}
	}
}
