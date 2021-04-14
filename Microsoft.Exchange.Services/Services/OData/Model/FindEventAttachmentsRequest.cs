using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Read")]
	[AllowedOAuthGrant("Calendars.Write")]
	internal class FindEventAttachmentsRequest : FindAttachmentsRequest
	{
		public FindEventAttachmentsRequest(ODataContext odataContext) : base(odataContext)
		{
		}
	}
}
