using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	internal class UpdateCalendarRequest : UpdateEntityRequest<Calendar>
	{
		public UpdateCalendarRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new UpdateCalendarCommand(this);
		}
	}
}
