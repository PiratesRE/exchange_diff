using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Write")]
	internal class CopyMessageRequest : CopyOrMoveEntityRequest<Message>
	{
		public CopyMessageRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new CopyMessageCommand(this);
		}
	}
}
