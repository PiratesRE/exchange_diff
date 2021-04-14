using System;
using System.IO;
using System.Net;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration
{
	internal interface IHolidayCalendarsService
	{
		void GetResource(WebRequest request, Action<Stream> responseProcessingDelegate);
	}
}
