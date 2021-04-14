using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	internal class UpdateEventRequest : UpdateEntityRequest<Event>
	{
		public UpdateEventRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new UpdateEventCommand(this);
		}
	}
}
