using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Contacts.Write")]
	internal class DeleteContactAttachmentRequest : DeleteAttachmentRequest
	{
		public DeleteContactAttachmentRequest(ODataContext odataContext) : base(odataContext)
		{
		}
	}
}
