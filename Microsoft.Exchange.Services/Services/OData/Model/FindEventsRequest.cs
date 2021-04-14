using System;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	[AllowedOAuthGrant("Calendars.Read")]
	internal class FindEventsRequest : FindEntitiesRequest<Event>
	{
		public FindEventsRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public string CalendarId { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			if (base.ODataContext.ODataPath.ParentOfEntitySegment is KeySegment && base.ODataContext.ODataPath.ParentOfEntitySegment.EdmType.Equals(Calendar.EdmEntityType))
			{
				this.CalendarId = base.ODataContext.ODataPath.ParentOfEntitySegment.GetIdKey();
				return;
			}
			this.CalendarId = null;
		}

		public override ODataCommand GetODataCommand()
		{
			return new FindEventsCommand(this);
		}

		public override void Validate()
		{
			base.Validate();
		}
	}
}
