using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Write")]
	internal class DeleteMessageAttachmentRequest : DeleteAttachmentRequest
	{
		public DeleteMessageAttachmentRequest(ODataContext odataContext) : base(odataContext)
		{
		}
	}
}
