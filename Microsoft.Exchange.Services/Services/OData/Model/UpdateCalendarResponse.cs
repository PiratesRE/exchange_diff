using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class UpdateCalendarResponse : UpdateEntityResponse<Calendar>
	{
		public UpdateCalendarResponse(UpdateCalendarRequest request) : base(request)
		{
		}
	}
}
