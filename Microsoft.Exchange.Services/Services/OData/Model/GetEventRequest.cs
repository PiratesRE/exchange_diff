using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Read")]
	[AllowedOAuthGrant("Calendars.Write")]
	internal class GetEventRequest : GetEntityRequest<Event>
	{
		public GetEventRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new GetEventCommand(this);
		}
	}
}
