using System;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration
{
	internal interface IEndpointInformationRetriever
	{
		EndpointInformation FetchEndpointInformation();
	}
}
