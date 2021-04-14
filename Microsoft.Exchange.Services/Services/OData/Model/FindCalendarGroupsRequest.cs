using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	[AllowedOAuthGrant("Calendars.Read")]
	internal class FindCalendarGroupsRequest : FindEntitiesRequest<CalendarGroup>
	{
		public FindCalendarGroupsRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new FindCalendarGroupsCommand(this);
		}
	}
}
