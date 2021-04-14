using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Read")]
	[AllowedOAuthGrant("Calendars.Write")]
	internal class GetEventAttachmentRequest : GetAttachmentRequest
	{
		public GetEventAttachmentRequest(ODataContext odataContext) : base(odataContext)
		{
		}
	}
}
