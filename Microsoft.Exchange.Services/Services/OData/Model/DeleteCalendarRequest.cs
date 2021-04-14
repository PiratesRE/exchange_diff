using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	internal class DeleteCalendarRequest : DeleteEntityRequest<Calendar>
	{
		public DeleteCalendarRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new DeleteCalendarCommand(this);
		}
	}
}
