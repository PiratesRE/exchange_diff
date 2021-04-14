using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	internal class DeleteEventAttachmentRequest : DeleteAttachmentRequest
	{
		public DeleteEventAttachmentRequest(ODataContext odataContext) : base(odataContext)
		{
		}
	}
}
