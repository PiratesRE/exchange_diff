using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	internal class DeleteEventRequest : DeleteEntityRequest<Event>
	{
		public DeleteEventRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public override ODataCommand GetODataCommand()
		{
			return new DeleteEventCommand(this);
		}
	}
}
