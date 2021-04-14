using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Read")]
	[AllowedOAuthGrant("Mail.Write")]
	internal class GetMessageAttachmentRequest : GetAttachmentRequest
	{
		public GetMessageAttachmentRequest(ODataContext odataContext) : base(odataContext)
		{
		}
	}
}
