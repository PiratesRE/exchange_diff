using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class ProxyServerWithMinimumRequiredVersionNotFound : AvailabilityException
	{
		public ProxyServerWithMinimumRequiredVersionNotFound(EmailAddress requester, int serverVersion, int minimumServerVersion) : base(ErrorConstants.E14orHigherProxyServerNotFound, Strings.descMinimumRequiredVersionProxyServerNotFound(serverVersion, minimumServerVersion, requester.ToString()))
		{
		}
	}
}
