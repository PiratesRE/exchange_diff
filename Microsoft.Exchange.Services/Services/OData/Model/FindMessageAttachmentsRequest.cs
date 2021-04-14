using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Mail.Read")]
	[AllowedOAuthGrant("Mail.Write")]
	internal class FindMessageAttachmentsRequest : FindAttachmentsRequest
	{
		public FindMessageAttachmentsRequest(ODataContext odataContext) : base(odataContext)
		{
		}
	}
}
