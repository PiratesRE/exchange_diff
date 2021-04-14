using System;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration
{
	internal interface IEndpointInformationRetrieverFactory
	{
		IEndpointInformationRetriever Create(Uri baseUrl, int timeout = 30000);
	}
}
