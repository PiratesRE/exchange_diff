using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Contacts.Write")]
	[AllowedOAuthGrant("Contacts.Read")]
	internal class GetContactAttachmentRequest : GetAttachmentRequest
	{
		public GetContactAttachmentRequest(ODataContext odataContext) : base(odataContext)
		{
		}
	}
}
