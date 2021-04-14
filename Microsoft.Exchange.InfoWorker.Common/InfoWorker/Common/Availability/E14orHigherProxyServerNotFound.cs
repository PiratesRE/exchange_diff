using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class E14orHigherProxyServerNotFound : AvailabilityException
	{
		public E14orHigherProxyServerNotFound(EmailAddress requester, int minimumServerVersion) : base(ErrorConstants.E14orHigherProxyServerNotFound, Strings.descE14orHigherProxyServerNotFound(requester.ToString(), minimumServerVersion))
		{
		}
	}
}
