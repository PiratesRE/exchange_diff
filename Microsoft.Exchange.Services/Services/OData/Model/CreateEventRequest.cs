using System;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	internal class CreateEventRequest : CreateEntityRequest<Event>
	{
		public CreateEventRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public string CalendarId { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			if (base.ODataContext.ODataPath.EntitySegment is NavigationPropertySegment)
			{
				if (base.ODataContext.ODataPath.ParentOfEntitySegment is KeySegment && base.ODataContext.ODataPath.ParentOfEntitySegment.EdmType.Equals(Calendar.EdmEntityType))
				{
					this.CalendarId = base.ODataContext.ODataPath.ParentOfEntitySegment.GetIdKey();
					return;
				}
				if (base.ODataContext.ODataPath.ParentOfEntitySegment is NavigationPropertySegment)
				{
					this.CalendarId = null;
				}
			}
		}

		public override ODataCommand GetODataCommand()
		{
			return new CreateEventCommand(this);
		}

		public override void Validate()
		{
			base.Validate();
		}
	}
}
