using System;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	[AllowedOAuthGrant("Calendars.Write")]
	internal class CreateCalendarRequest : CreateEntityRequest<Calendar>
	{
		public CreateCalendarRequest(ODataContext odataContext) : base(odataContext)
		{
		}

		public string CalendarGroupId { get; protected set; }

		public override void LoadFromHttpRequest()
		{
			base.LoadFromHttpRequest();
			if (base.ODataContext.ODataPath.EntitySegment is NavigationPropertySegment && base.ODataContext.ODataPath.ParentOfEntitySegment is KeySegment && base.ODataContext.ODataPath.ParentOfEntitySegment.EdmType.Equals(CalendarGroup.EdmEntityType))
			{
				this.CalendarGroupId = base.ODataContext.ODataPath.ParentOfEntitySegment.GetIdKey();
			}
		}

		public override ODataCommand GetODataCommand()
		{
			return new CreateCalendarCommand(this);
		}
	}
}
