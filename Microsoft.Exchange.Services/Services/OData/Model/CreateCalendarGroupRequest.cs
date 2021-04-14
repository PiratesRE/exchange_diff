using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	internal class CreateCalendarGroupRequest : CreateEntityRequest<CalendarGroup>
	{
		public CreateCalendarGroupRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new CreateCalendarGroupCommand(this);
		}
	}
}
