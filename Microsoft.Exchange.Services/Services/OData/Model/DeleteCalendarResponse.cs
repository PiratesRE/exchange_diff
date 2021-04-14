using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DeleteCalendarResponse : DeleteEntityResponse<Calendar>
	{
		public DeleteCalendarResponse(DeleteCalendarRequest request) : base(request)
		{
		}
	}
}
