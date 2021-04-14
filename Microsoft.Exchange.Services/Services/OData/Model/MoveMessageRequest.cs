using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Write")]
	internal class MoveMessageRequest : CopyOrMoveEntityRequest<Message>
	{
		public MoveMessageRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new MoveMessageCommand(this);
		}
	}
}
