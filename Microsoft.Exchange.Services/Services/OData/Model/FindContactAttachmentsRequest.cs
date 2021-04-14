using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Contacts.Write")]
	[AllowedOAuthGrant("Contacts.Read")]
	internal class FindContactAttachmentsRequest : FindAttachmentsRequest
	{
		public FindContactAttachmentsRequest(ODataContext odataContext) : base(odataContext)
		{
		}
	}
}
