using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	internal class DeleteCalendarGroupRequest : DeleteEntityRequest<CalendarGroup>
	{
		public DeleteCalendarGroupRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new DeleteCalendarGroupCommand(this);
		}
	}
}
