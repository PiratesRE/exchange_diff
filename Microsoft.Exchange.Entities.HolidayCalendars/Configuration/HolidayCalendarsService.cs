using System;
using System.IO;
using System.Net;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration
{
	internal class HolidayCalendarsService : IHolidayCalendarsService
	{
		public void GetResource(WebRequest request, Action<Stream> responseProcessingDelegate)
		{
			using (WebResponse response = request.GetResponse())
			{
				responseProcessingDelegate(response.GetResponseStream());
			}
		}
	}
}
