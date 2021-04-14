using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	[AllowedOAuthGrant("Calendars.Read")]
	internal class GetCalendarGroupRequest : GetEntityRequest<CalendarGroup>
	{
		public GetCalendarGroupRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new GetCalendarGroupCommand(this);
		}
	}
}
