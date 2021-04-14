using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class GetCalendarResponse : GetEntityResponse<Calendar>
	{
		public GetCalendarResponse(GetCalendarRequest request) : base(request)
		{
		}
	}
}
