using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	[AllowedOAuthGrant("Calendars.Read")]
	internal class GetCalendarRequest : GetEntityRequest<Calendar>
	{
		public GetCalendarRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new GetCalendarCommand(this);
		}
	}
}
