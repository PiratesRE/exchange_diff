using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class CreateCalendarResponse : CreateEntityResponse<Calendar>
	{
		public CreateCalendarResponse(CreateCalendarRequest request) : base(request)
		{
		}
	}
}
