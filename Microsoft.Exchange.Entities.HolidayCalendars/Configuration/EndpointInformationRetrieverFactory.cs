using System;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration
{
	internal class EndpointInformationRetrieverFactory : IEndpointInformationRetrieverFactory
	{
		public IEndpointInformationRetriever Create(Uri baseUrl, int timeout = 30000)
		{
			return new EndpointInformationRetriever(baseUrl, timeout, null);
		}
	}
}
