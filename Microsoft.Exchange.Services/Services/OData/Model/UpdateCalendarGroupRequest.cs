using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	internal class UpdateCalendarGroupRequest : UpdateEntityRequest<CalendarGroup>
	{
		public UpdateCalendarGroupRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new UpdateCalendarGroupCommand(this);
		}
	}
}
